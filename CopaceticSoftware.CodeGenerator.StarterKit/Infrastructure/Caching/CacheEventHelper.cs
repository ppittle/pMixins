//----------------------------------------------------------------------- 
// <copyright file="CacheEventHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 13, 2014 4:54:11 PM</date> 
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
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    /// <summary>
    /// Sits on top the <see cref="IVisualStudioEventProxy"/> and provides
    /// more intelligent events for evicting cached files
    /// </summary>
    public interface ICacheEventHelper
    {
        event EventHandler<EvictFromCacheEventArgs> OnEvictFromCache;
        event EventHandler<EventArgs> OnClearCache;
    }

    public class EvictFromCacheEventArgs : EventArgs
    {
        public string FileName { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating if the file on disk
        /// has changed.  If this is false it generally means that only
        /// the solution context surrounding this file has changed, ie, it's
        /// compilation needs to be regenerated.
        /// </summary>
        public bool FileOnDiskHasChanged { get; set; }

        public EvictFromCacheEventArgs(string filename)
        {
            FileName = filename;
            FileOnDiskHasChanged = true;
        }
    }

    public class CacheEventHelper : ICacheEventHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler<EvictFromCacheEventArgs> OnEvictFromCache;
        public event EventHandler<EventArgs> OnClearCache;

        public CacheEventHelper(IVisualStudioEventProxy visualStudioEventProxy, ICodeGeneratorDependencyManager codeGeneratorDependencyManager, ISolutionContext solutionContext)
        {
            AddEmptyEventHandlers();

            WireUpVisualStudioEvents(visualStudioEventProxy, codeGeneratorDependencyManager, solutionContext);
        }

        private void WireUpVisualStudioEvents(IVisualStudioEventProxy visualStudioEventProxy,
            ICodeGeneratorDependencyManager codeGeneratorDependencyManager, ISolutionContext solutionContext)
        {
            visualStudioEventProxy.OnSolutionClosing +=
                (sender, args) =>
                {
                    _log.Info("Solution closing.  Firing OnClearCache");
                    OnClearCache(this, new EventArgs());
                };

            visualStudioEventProxy.OnProjectAdded +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectAdded - Evict [{0}]", solutionContext.SolutionFileName);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(solutionContext.SolutionFileName));
                };

            visualStudioEventProxy.OnProjectRemoved +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectRemoved - Evict [{0}]", args.ProjectFullPath);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.ProjectFullPath));

                    _log.InfoFormat("OnProjectRemoved - Evict [{0}]", solutionContext.SolutionFileName);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(solutionContext.SolutionFileName));
                };

            visualStudioEventProxy.OnProjectReferenceAdded +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectReferenceAdded - Evict [{0}]", args.ProjectFullPath);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.ProjectFullPath));

                    //TODO - Evict Project Items - Compilation needs to be updated?
                };

            visualStudioEventProxy.OnProjectReferenceRemoved +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectReferenceRemoved - Evict [{0}]", args.ProjectFullPath);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.ProjectFullPath));

                    //TODO - Evict Project Items - Compilation needs to be updated?
                };

            visualStudioEventProxy.OnProjectItemSaved +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectItemSaved - Evict [{0}]", args.ClassFullPath);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.ClassFullPath));

                    codeGeneratorDependencyManager
                        .GetFilesThatDependOn(args.ClassFullPath)
                        .Map(f =>
                        {
                            _log.InfoFormat("OnProjectItemSaved - Evict Dependency [{0}]", f.FileName);

                            OnEvictFromCache(this, new EvictFromCacheEventArgs(f.FileName)
                            {
                                FileOnDiskHasChanged = false
                            });
                        });
                };

            visualStudioEventProxy.OnProjectItemRemoved +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectItemRemoved - Evict [{0}]", args.ClassFullPath);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.ClassFullPath));
                };

            visualStudioEventProxy.OnProjectItemRenamed +=
                (sender, args) =>
                {
                    _log.InfoFormat("OnProjectItemRemoved - Evict [{0}]", args.OldClassFileName);

                    OnEvictFromCache(this, new EvictFromCacheEventArgs(args.OldClassFileName));
                };
        }

        private void AddEmptyEventHandlers()
        {
            OnEvictFromCache += (sender, args) => { };
            OnClearCache += (sender, args) => { };
        }
    }
}
