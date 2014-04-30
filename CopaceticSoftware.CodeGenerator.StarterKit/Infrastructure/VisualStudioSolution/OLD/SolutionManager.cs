//----------------------------------------------------------------------- 
// <copyright file="SolutionManager.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 5:48:10 PM</date> 
// Licensed under the Apache License, Version 2.0,
// you may not use this file except in compliance with this License.
//  
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright> 
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD.NRefactory;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD
{
    public interface ISolutionManager : IDisposable
    {
        Solution Solution { get; }

        ISolutionExtender SolutionExtender { get; }

        CSharpFile AddOrUpdateCodeGeneratorFileSource(string projectFilePath, string codeFilePath, string sourceCode);

        /// <summary>
        /// Insturcts the <see cref="ISolutionManager"/> to reparse any parts of a Solution
        /// that are out-of-date
        /// </summary>
        void EnsureSolutionIsFullyParsed();

        void EnsureSolutionIsFullyParsedAsync();
    }


    public class SolutionManager : ISolutionManager
    {
        #region Data Members

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object SolutionExtenderLock = new object();
        private static readonly object EventQueueLock = new object();


        public ISolutionExtender SolutionExtender { get; set; }

        /// <summary>
        /// Keep a reference to the <see cref="IVisualStudioEventProxy"/>
        /// to make sure it doesn't get GC'd
        /// </summary>
        private readonly IVisualStudioEventProxy _visualStudioEvents;

        /// <summary>
        /// A <see cref="IList{T}"/> for Visual Studio events that still need 
        /// to be applied before the <see cref="SolutionExtender"/> is up-to-date.
        /// </summary>
        private readonly IList<VisualStudioEventArgs> _pendingVisualStudioEvents = new List<VisualStudioEventArgs>();

        private readonly IList<string> _codeGeneratedFileNames = new List<string>();

        #endregion

        #region Constructor

        public SolutionManager(string solutionName, IVisualStudioEventProxy visualStudioEvents)
            : this(new Solution(solutionName), visualStudioEvents)
        {
        }

        public SolutionManager(Solution solution, IVisualStudioEventProxy visualStudioEvents)
            : this(new SolutionExtender(solution), visualStudioEvents)
        {
        }

        public SolutionManager(ISolutionExtender solutionExtender, IVisualStudioEventProxy visualStudioEvents)
        {
            Ensure.ArgumentNotNull(solutionExtender, "solutionExtender");
            Ensure.ArgumentNotNull(visualStudioEvents, "visualStudioEvents");

            SolutionExtender = solutionExtender;
            _visualStudioEvents = visualStudioEvents;

            WireUpVisualStudioEvents(_visualStudioEvents);
        }

        #endregion

        #region EnsureSolutionIsFullyParsed

        public void EnsureSolutionIsFullyParsed()
        {
            EnsureSolutionIsFullyParsedInternal();
        }

        public void EnsureSolutionIsFullyParsedAsync()
        {
            Task.Factory.StartNew(EnsureSolutionIsFullyParsedInternal);
        }

        private void EnsureSolutionIsFullyParsedInternal()
        {
            lock (SolutionExtenderLock)
            {
                #region Log Preamble

                _log.InfoFormat(
                    "EnsureSolutionIsFullyParsed - Beginning processing of [{0}] queued visual studio events.",
                    _pendingVisualStudioEvents.Count);

                #endregion

                IList<VisualStudioEventArgs> eventsWorkingQueue;

                try
                {
                    lock (EventQueueLock)
                    {
                        eventsWorkingQueue = _pendingVisualStudioEvents.Select(a => a.DeepCopyLocal()).ToList();

                        _pendingVisualStudioEvents.Clear();
                    }

                    eventsWorkingQueue = PruneAndOrderEvents(eventsWorkingQueue);
                }
                catch (Exception e)
                {
                    _log.Error("Exception copying and pruning pendingVisualStudioEvents: " + e.Message, e);
                    return;
                }

                foreach (var vsEvent in eventsWorkingQueue)
                {
                    #region Log Event

                    _log.InfoFormat("Processing event [{0}]", vsEvent.GetDebugString());

                    #endregion

                    try
                    {
                        ProcessEventSafe(vsEvent);
                    }
                    catch (Exception e)
                    {
                        _log.Error(string.Format(
                            "Exception processing event [{0}]", vsEvent.GetDebugString()), e);
                    }
                }

                #region Log Afterword

                _log.Info("EnsureSolutionIsFullyParsed - Has Completed");

                #endregion
            }
        }

        private IList<VisualStudioEventArgs> PruneAndOrderEvents(IList<VisualStudioEventArgs> eventsWorkingQueue)
        {
            var returnEvents = new List<VisualStudioEventArgs>(eventsWorkingQueue);

            //For any Project events, remove any other events that deal with that Project 
            //(we're going to have to completly rescan the project anyway)
            returnEvents.RemoveMany(
                eventsWorkingQueue
                    .OfType<VisualStudioProjectEventArgs>()
                    .SelectMany(projectEvent =>
                        eventsWorkingQueue
                            .Where(
                                projectItemEvent => projectItemEvent.ProjectFullPath == projectEvent.ProjectFullPath
                                                    && !(projectItemEvent is VisualStudioProjectEventArgs))));

            return returnEvents;
        }

        private void ProcessEventSafe(VisualStudioEventArgs vsEvent)
        {
            if (vsEvent is VisualStudioBuildEventArgs)
                return;

            if (vsEvent is ProjectAddedEventArgs || vsEvent is ProjectReferenceAddedEventArgs
                || vsEvent is ProjectReferenceRemovedEventArgs)
            {
                SolutionExtender.AddOrUpdateProject(vsEvent.ProjectFullPath);
                return;
            }

            if (vsEvent is ProjectRemovedEventArgs)
            {
                SolutionExtender.RemoveProject(SolutionExtender.GetProjectByFilePath(vsEvent.ProjectFullPath));
                return;
            }

            if (vsEvent is VisualStudioClassEventArgs)
            {
                var classEvent = vsEvent as VisualStudioClassEventArgs;

                if (!classEvent.IsCSharpFile())
                    return;

                if (classEvent is ProjectItemAddedEventArgs)
                {
                    SolutionExtender.AddFileToProject(classEvent.ProjectFullPath, classEvent.ClassFullPath);
                    return;
                }

                if (classEvent is ProjectItemRemovedEventArgs)
                {
                    SolutionExtender.RemoveCSharpFileFromProject(
                        SolutionExtender.GetProjectByFilePath(vsEvent.ProjectFullPath),
                        classEvent.ClassFullPath);

                    return;
                }

                if (classEvent is ProjectItemRenamedEventArgs)
                {
                    var origFileName = ((ProjectItemRenamedEventArgs) classEvent).OldClassFileName;
                    var project = SolutionExtender.GetProjectByFilePath(classEvent.ProjectFullPath);

                    if (null == project)
                        throw new Exception(string.Format(Strings.ExceptionCouldNotFindProjectWithFullName,
                            classEvent.ProjectFullPath));

                    SolutionExtender.RemoveCSharpFileFromProject(project, origFileName);
                    SolutionExtender.AddFileToProject(classEvent.ProjectFullPath, classEvent.ClassFullPath);

                    return;
                }

                if (classEvent is ProjectItemSavedEventArgs)
                {
                    var project = SolutionExtender.GetProjectByFilePath(classEvent.ProjectFullPath);

                    if (null == project)
                        throw new Exception(string.Format(Strings.ExceptionCouldNotFindProjectWithFullName,
                            classEvent.ProjectFullPath));

                    SolutionExtender.RemoveCSharpFileFromProject(project, classEvent.ClassFullPath);
                    SolutionExtender.AddFileToProject(classEvent.ProjectFullPath, classEvent.ClassFullPath);
                }
            }

            //If we made it this far we don't know how to handle this type of event.
            throw new Exception(
                string.Format(
                    "ProcessEventSafe() does not know how to handle event of type [{0}].  This is a bug and the method should be updated.",
                    vsEvent.GetType()));
        }

        #endregion

        #region Visual Studio Events Wiring/Queueing/Handling

        private void WireUpVisualStudioEvents(IVisualStudioEventProxy proxy)
        {
            proxy.OnProjectAdded += QueueVisualStudioEvent;
            proxy.OnProjectRemoved += QueueVisualStudioEvent;
            proxy.OnProjectReferenceAdded += QueueVisualStudioEvent;
            proxy.OnProjectReferenceRemoved += QueueVisualStudioEvent;
            proxy.OnProjectItemAdded += QueueVisualStudioEvent;
            proxy.OnProjectItemRemoved += QueueVisualStudioEvent;
            proxy.OnProjectItemRenamed += QueueVisualStudioEvent;
            proxy.OnProjectItemSaved += QueueVisualStudioEvent;

            proxy.OnProjectItemOpened += (sender, args) =>
                                         {
                                             if (!_codeGeneratedFileNames.Contains(args.ClassFullPath))
                                                 return;

                                             _log.InfoFormat(
                                                 "Eagerly 'EnsureSolutionIsFullyParsedAsync' in response to a File Opened event for [{0}]",
                                                 args.ClassFullPath);

                                             EnsureSolutionIsFullyParsedAsync();
                                         };
        }

        /// <summary>
        /// Makes sure <paramref name="eventArgs"/> is not null
        /// and logs the event to <see cref="_log"/>
        /// </summary>
        /// <returns><c>False</c> if <paramref name="eventArgs"/> is <c>null</c>, <c>True</c> otherwise.</returns>
        private void QueueVisualStudioEvent(object sender, VisualStudioEventArgs eventArgs)
        {
            #region Null Guard Check and Logging

            if (null == eventArgs)
            {
                _log.Error("Received a [null] event in OnVisualStudioEvents");
                return;
            }

            var senderId = (null == sender) ? "<null sender>" : sender.GetType().Name;
            _log.InfoFormat("[{0}] sent event: {1}", senderId, eventArgs.GetDebugString());

            #endregion

            if (eventArgs is VisualStudioClassEventArgs && !(eventArgs as VisualStudioClassEventArgs).IsCSharpFile())
                return;

            lock (EventQueueLock)
            {
                _pendingVisualStudioEvents.Add(eventArgs);
            }
        }

        #endregion

        #region Wrapped ISolutionManager Members

        public Solution Solution
        {
            get { return SolutionExtender.Solution; }
        }

        public CSharpFile AddOrUpdateCodeGeneratorFileSource(string projectFilePath, string codeFilePath,
            string sourceCode)
        {
            lock (SolutionExtenderLock)
            {
                if (!_codeGeneratedFileNames.Contains(codeFilePath))
                    _codeGeneratedFileNames.Add(codeFilePath);

                return SolutionExtender.AddOrUpdateProjectItemFile(projectFilePath, codeFilePath, sourceCode);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (null != _visualStudioEvents)
                _visualStudioEvents.Dispose();
        }

        #endregion
    }
}
