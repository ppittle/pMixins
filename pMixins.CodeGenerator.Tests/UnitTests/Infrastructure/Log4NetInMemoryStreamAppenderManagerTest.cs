//----------------------------------------------------------------------- 
// <copyright file="Log4NetInMemoryStreamAppenderManagerTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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

using System.Linq;
using System.Reflection;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.Tests.Common;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.UnitTests.Infrastructure
{
    [TestFixture]
    public class Log4NetInMemoryStreamAppenderManagerTest : TestBase
    {
        private const string _logMessage = "Test Log Message";

        private readonly log4net.ILog _log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Log4NetInMemoryStreamAppenderManager _log4NetInMemoryStreamAppenderManager;

        protected override void Establish_context()
        {
            //Note: _log4NetInMemoryStreamAppenderManager
            //must be initialized here, otherwise the TestBase
            //constructor will reinitialize the log4net environment
            _log4NetInMemoryStreamAppenderManager = 
                new Log4NetInMemoryStreamAppenderManager();

            _log.Info(_logMessage);
        }

        protected override void Cleanup()
        {
            _log4NetInMemoryStreamAppenderManager.Dispose();
        }

        [Test]
        public void Can_Read_Logs()
        {
            _log4NetInMemoryStreamAppenderManager.GetLoggingEvents(LoggingVerbosity.All).Count()
                .ShouldEqual(1);

            _log4NetInMemoryStreamAppenderManager.GetLoggingEvents(LoggingVerbosity.All).First().RenderedMessage
                .ShouldContain(_logMessage);
        }

        [Test]
        public void Rendered_Log_Message_Should_Start_With_Comment()
        {
            _log4NetInMemoryStreamAppenderManager.GetRenderedLoggingEvents(LoggingVerbosity.All).First()
                .ShouldStartWith("//");
        }

        [Test]
        public void LoggingVerbosity_Flag_Should_Be_Respected()
        {
            _log4NetInMemoryStreamAppenderManager.GetLoggingEvents(LoggingVerbosity.Warning).Count()
                .ShouldEqual(0);
        }
    }
}
