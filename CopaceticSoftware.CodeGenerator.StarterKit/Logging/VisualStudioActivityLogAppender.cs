//----------------------------------------------------------------------- 
// <copyright file="VisualStudioActivityLogAppender.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 9, 2014 3:25:56 PM</date> 
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
using log4net.Appender;
using log4net.Core;
using Microsoft.VisualStudio.Shell.Interop;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/bb166359.aspx
    /// </summary>
    public class VisualStudioActivityLogAppender : AppenderSkeleton
    {
        private readonly IVsActivityLog _log;
        public string Source { get; set; }

        public VisualStudioActivityLogAppender(IServiceProvider serviceProvider)
        {
            _log = serviceProvider.GetService(typeof(SVsActivityLog)) as IVsActivityLog;

            Source = typeof (VisualStudioActivityLogAppender).Name;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_log == null) 
                return;

            var entryType = __ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION;

            if (loggingEvent.Level == Level.Critical ||
                loggingEvent.Level == Level.Emergency ||
                loggingEvent.Level == Level.Fatal ||
                loggingEvent.Level == Level.Error)
            {
                entryType = __ACTIVITYLOG_ENTRYTYPE.ALE_ERROR;
            }
            else if (loggingEvent.Level == Level.Warn)
                entryType = __ACTIVITYLOG_ENTRYTYPE.ALE_WARNING;

            _log.LogEntry(
                (UInt32)entryType,
                Source,
                RenderLoggingEvent(loggingEvent));
        }
    }
}
