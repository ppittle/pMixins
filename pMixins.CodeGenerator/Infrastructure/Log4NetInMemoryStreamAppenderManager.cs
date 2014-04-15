//----------------------------------------------------------------------- 
// <copyright file="Log4NetInMemoryStreamAppenderManager.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, November 10, 2013 12:26:14 AM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure
{
    /// <summary>
    /// Adds an In Memory Appender to the Log4Net logging infrastructure
    /// and provides a mechanism for retrieving log messages.
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/1519728/programmatically-adding-and-removing-log-appenders-in-log4net
    /// </remarks>
    public class Log4NetInMemoryStreamAppenderManager : IDisposable
    {
        private readonly IAppenderAttachable _loggerRoot;
        private readonly MemoryAppender _memoryAppender;
        
        public Log4NetInMemoryStreamAppenderManager()
        {
            _loggerRoot = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;

            _memoryAppender = new MemoryAppender
                                  {
                                      Threshold = Level.All,
                                      Name = this.GetType().Name,
                                      Layout = new PatternLayout("{message}")
                                  };

            if (_loggerRoot != null)
                _loggerRoot.AddAppender(_memoryAppender);
        }

        public IEnumerable<LoggingEvent> GetLoggingEvents(LoggingVerbosity verbosity)
        {
            Level logginglevel;
            #region Convert LoggingVerbosity to Level
            switch(verbosity)
            {
                case LoggingVerbosity.Error:
                    logginglevel = Level.Error;
                    break;
                case LoggingVerbosity.Warning:
                    logginglevel = Level.Warn;
                    break;
                case LoggingVerbosity.Info:
                    logginglevel = Level.Info;
                    break;
                default:
                    logginglevel = Level.All;
                    break;
            }
            #endregion

            return _memoryAppender.GetEvents()
                .Where(x => x.Level >= logginglevel);
        }

        public IEnumerable<string> GetRenderedLoggingEvents(LoggingVerbosity verbosity)
        {
            return GetLoggingEvents(verbosity)
                .Select(loggingEvent =>
                        string.Format("// [{0}] {1} {2} - {3}",
                                      loggingEvent.TimeStamp,
                                      loggingEvent.Level,
                                      loggingEvent.LoggerName,
                                      loggingEvent.RenderedMessage));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_loggerRoot != null)
                _loggerRoot.RemoveAppender(_memoryAppender);
        }
    }
}
