//----------------------------------------------------------------------- 
// <copyright file="VisualStudioEventProxyLogger.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 20, 2014 1:37:58 PM</date> 
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

using System.Reflection;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure
{
    public class VisualStudioEventProxyLogger
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public VisualStudioEventProxyLogger(IVisualStudioEventProxy eventProxy)
        {
            eventProxy.OnBuildBegin += (o, a) => LogEventArgs(a);
            eventProxy.OnBuildDone += (o, a) => LogEventArgs(a);
            eventProxy.OnCodeGenerated += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectAdded += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemAdded += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemClosed += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemOpened += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemRemoved += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemRenamed += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemSaveComplete += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectItemSaved += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectReferenceAdded += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectReferenceRemoved += (o, a) => LogEventArgs(a);
            eventProxy.OnProjectRemoved += (o, a) => LogEventArgs(a);
            eventProxy.OnSolutionClosing += (o, a) => _log.Debug("Solution Closing");
            eventProxy.OnSolutionOpening += (o, a) => _log.Debug("Solution Opening");
        }

        private void LogEventArgs(CodeGeneratedEventArgs codeGeneratedEventArgs)
        {
            _log.DebugFormat("Code Generated Event");
        }

        private void LogEventArgs(VisualStudioEventArgs a)
        {
            _log.Debug(a.GetDebugString());
        }
    }
}
