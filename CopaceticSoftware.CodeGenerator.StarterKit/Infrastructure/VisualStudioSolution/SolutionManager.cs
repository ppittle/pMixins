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
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{

    public interface ISolutionManager : IDisposable
    {
        Task EnsureSolutionIsUpToDate();
    }

    public class SolutionManager : ISolutionManager
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioEventProxy _visualStudioEventProxy;

        protected readonly ConcurrentSpecializedList<VisualStudioEventArgs> _visualStudioEventQueue
            = new ConcurrentSpecializedList<VisualStudioEventArgs>();

        protected readonly ConcurrentBag<string> _codeGeneratedFileNames = new ConcurrentBag<string>(); 

        public Solution Solution { get; set; }

        public SolutionManager(Solution solution, IVisualStudioEventProxy visualStudioEventProxy)
        {
            Solution = solution;
            _visualStudioEventProxy = visualStudioEventProxy;

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
                if (!_codeGeneratedFileNames.Any(f => f.Equals(args.ClassFullPath)))
                    return;

                _log.InfoFormat(
                    "Eagerly 'EnsureSolutionIsUpToDate' in response to a File Opened event for [{0}]",
                    args.ClassFullPath);

                EnsureSolutionIsUpToDate();
            };
        }

        public Task EnsureSolutionIsUpToDate()
        {
            return new TaskFactory().StartNew(
                () => new SolutionSyncer().SyncSolution(
                    Solution, 
                    _visualStudioEventQueue.CopyToArrayAndClear()));
        }

        protected virtual void QueueVisualStudioEvent(object sender, VisualStudioEventArgs eventArgs)
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
        public void SyncSolution(Solution solution, IEnumerable<VisualStudioEventArgs> visualStudioEvents)
        {
            
        }
    }
}
