//----------------------------------------------------------------------- 
// <copyright file="SolutionManager.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Collections;
using JetBrains.Annotations;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface ISolutionManager : IDisposable
    {
        void LoadSolution(string solutionFileName);

        void RegisterCodeGeneratorResponse(CodeGeneratorResponse response);

        Task EnsureSolutionIsUpToDate();

        IEnumerable<CSharpFile> LoadCSharpFiles(IEnumerable<RawSourceFile> rawSourceFiles);
    }

    public class SolutionManager : ISolutionManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioEventProxy _visualStudioEventProxy;
        private readonly ISolutionFactory _solutionFactory;

        protected readonly ConcurrentSpecializedList<VisualStudioEventArgs> _visualStudioEventQueue
            = new ConcurrentSpecializedList<VisualStudioEventArgs>();

        protected ConcurrentBag<CSharpFile> _codeGeneratedFiles = new ConcurrentBag<CSharpFile>(); 

        public Solution Solution { get; private set; }

        private bool _monitorVisualStudioEvents = false;

        public SolutionManager(IVisualStudioEventProxy visualStudioEventProxy, ISolutionFactory solutionFactory)
        {
            _visualStudioEventProxy = visualStudioEventProxy;
            _solutionFactory = solutionFactory;

            visualStudioEventProxy.OnProjectAdded += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectRemoved += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectReferenceAdded += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectReferenceRemoved += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectItemAdded += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectItemRemoved += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectItemRenamed += QueueVisualStudioEvent;
            visualStudioEventProxy.OnProjectItemSaved += QueueVisualStudioEvent;

            visualStudioEventProxy.OnProjectItemOpened += (sender, args) =>
            {
                if (!_monitorVisualStudioEvents)
                    return;

                if (!_codeGeneratedFiles.Any(f => f.FileName.Equals(args.ClassFullPath)))
                    return;

                _log.InfoFormat(
                    "Eagerly 'EnsureSolutionIsUpToDate' in response to a File Opened event for [{0}]",
                    args.ClassFullPath);

                EnsureSolutionIsUpToDate();
            };

            visualStudioEventProxy.OnProjectRemoved += (sender, args) =>
            {
                if (!_monitorVisualStudioEvents)
                    return;

                //Remove _codeGeneratedFiles for the Project
                _codeGeneratedFiles = new ConcurrentBag<CSharpFile>(
                    _codeGeneratedFiles.Where(
                        c => c.Project.FileName != args.ProjectFullPath));
            };

            visualStudioEventProxy.OnProjectItemRemoved += (sender, args) =>
            {
                if (!_monitorVisualStudioEvents)
                    return;

                //Remove the _projectItem in Code Generated Files
                _codeGeneratedFiles = new ConcurrentBag<CSharpFile>(
                    _codeGeneratedFiles.Where(
                        c => c.FileName != args.ClassFullPath));
            };

            //TODO: Update _codeGeneratedFiles OnProjectItemRenamed
        }

        public void LoadSolution(string solutionFileName)
        {
            _log.InfoFormat("Loading Solution [{0}]", solutionFileName);

            _monitorVisualStudioEvents = false;

            Solution = _solutionFactory.BuildSolution(solutionFileName);

            _visualStudioEventQueue.Clear();

            _monitorVisualStudioEvents = true;

            _log.InfoFormat("Completed Loading Solution [{0}]", solutionFileName);
        }

        public void RegisterCodeGeneratorResponse(CodeGeneratorResponse response)
        {
            _codeGeneratedFiles.Add(response.CodeGeneratorContext.Source);
        }

        public Task EnsureSolutionIsUpToDate()
        {
            if (null == Solution)
                return new TaskFactory().StartNew(() => { });

            return new TaskFactory().StartNew(
                () => new SolutionSyncer().SyncSolution(
                    Solution, 
                    _visualStudioEventQueue.CopyToArrayAndClear()));
        }

        public IEnumerable<CSharpFile> LoadCSharpFiles(IEnumerable<RawSourceFile> rawSourceFiles)
        {
            throw new NotImplementedException();
        }

        protected virtual void QueueVisualStudioEvent(object sender, VisualStudioEventArgs eventArgs)
        {
            if (!_monitorVisualStudioEvents)
                return;

            #region Null Guard Check and Logging

            if (null == eventArgs)
            {
                _log.Error("Received a [null] event in OnVisualStudioEvents");
                return;
            }

            var senderId = (null == sender) ? "<null sender>" : sender.GetType().Name;
            _log.InfoFormat("[{0}] sent event: {1}", senderId, eventArgs.GetDebugString());

            #endregion

            if (eventArgs is VisualStudioClassEventArgs && 
                !(eventArgs as VisualStudioClassEventArgs).IsCSharpFile())
                return;

            _visualStudioEventQueue.Add(eventArgs);
        }

        public void Dispose()
        {
            _visualStudioEventProxy.Dispose();
        }
    }

    public class SolutionSyncer
    {
        public void SyncSolution(Solution solution, VisualStudioEventArgs[] visualStudioEvents)
        {
            if (null == visualStudioEvents || visualStudioEvents.Length == 0)
                return;
        }
    }
}
