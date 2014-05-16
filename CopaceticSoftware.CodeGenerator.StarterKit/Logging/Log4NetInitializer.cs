//----------------------------------------------------------------------- 
// <copyright file="Log4NetInitializer.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using ICSharpCode.NRefactory.Utils;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    public static class Log4NetInitializer
    {
        private static bool _isInitialized;

        private static object _lock = new object();

        public static void Initialize(IVisualStudioWriter visualStudioWriter, IServiceProvider serviceProvider)
        {
            if (_isInitialized)
                return;

            lock (_lock)
            {
                if (_isInitialized)
                    return;

                //http://stackoverflow.com/questions/650694/changing-the-log-level-programmaticaly-in-log4net
                var outputWindowAppender =
                    new VisualStudioOutputWindowAppender(visualStudioWriter)
                    {
                        Layout =
                            new PatternLayout(@"%date{HH:mm:ss,fff} %thread% %-5level [%logger{2}] %message%newline"),
                        Threshold = Level.Info
                    };

                outputWindowAppender.AddFilter(
                    new LevelAndLoggerFilter
                    {
                        LevelMax = Level.Fatal,
                        LevelMin = Level.Warn, 
                        LoggerToMatch = typeof(IPipelineStep<>).Namespace
                    });

                var activityLogAppender = new VisualStudioActivityLogAppender(serviceProvider)
                {
                    Layout =
                           new PatternLayout(@"%date{HH:mm:ss,fff} %thread% %-5level [%logger{2}] %message%newline"),
                    Threshold = Level.Info
                };

#if DEBUG
                var debugFileAppender = new RollingFileAppender
                {
                    AppendToFile = true,
                    Threshold = Level.All,
                    Layout = new PatternLayout(@"%date{HH:mm:ss,fff} %thread% %-5level [%logger{2}] %message%newline"),
                    File = @"c:\temp\pMixinsCodeGenerator.log",
                    DatePattern = "yyyyMMdd'.log'",
                    RollingStyle = RollingFileAppender.RollingMode.Date
                };
#endif

                log4net.Config.BasicConfigurator.Configure(
                    outputWindowAppender,
#if DEBUG
                    debugFileAppender,
#endif
                    activityLogAppender);

                _isInitialized = true;
            }
        }
    }
}
