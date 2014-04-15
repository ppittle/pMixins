//----------------------------------------------------------------------- 
// <copyright file="Log4NetMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 4:59:52 PM</date> 
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


using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class Log4NetMixin : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return

                    @"
                using log4net;
                using System.Reflection;

                namespace Test
                {
                    public class Log4NetMixin
                    {
                        protected ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                    }
                           
                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.Log4NetMixin))]
                    public partial class Target
                    {
                        public void MethodThatLogs()
                        {
                            Log.Info(""Hello World!"");
                        }
                    }                        
                }";
            }
        }

        [Test]
        public void CanCallMethodThatLogs()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.Target",
                "MethodThatLogs");
        }
    }
}
