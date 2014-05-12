//----------------------------------------------------------------------- 
// <copyright file="InterceptorWithMixinRequirementTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 3:47:47 PM</date> 
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

using System.Collections.Generic;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.ConversionOperators;
using CopaceticSoftware.pMixins.Interceptors;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    [TestFixture]
    public class 
        
        
        InterceptorWithMixinRequirementTest : GenerateCodeAndCompileTestBase
    {
        #region MixinInterceptor
        public class MockLogMixin
        {
            public MockLogMixin() { Messages = new List<string>(); }

            public IList<string> Messages { get; private set; }

            public void AddLogMessage(string message)
            {
                Messages.Add(message);
            }
        }

        [InterceptorMixinRequirement(Mixin = typeof(MockLogMixin))]
        public class InterceptorWithRequirement : MixinInterceptorBase
        {
            public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
            {
                eventArgs.Target.As<MockLogMixin>().AddLogMessage(
                    "OnBeforeMethod: " + eventArgs.MemberName);
            }

            public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
            {
                eventArgs.Target.As<MockLogMixin>().AddLogMessage(
                    "OnAfterMethod: " + eventArgs.MemberName);
            }
        }
        #endregion

        protected override string SourceCode
        {
            get
            {
                return
                    string.Format(
                        @"
                using System;

                namespace Test
                {{
                    public class Mixin
                    {{
                        public string Method()
                        {{
                            WasMethodCalled = true;

                            return ""Mixin"";
                        }}

                         public bool WasMethodCalled {{ get; private set; }}
                    }}

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.Mixin),
                        Interceptors = new Type[] {{ typeof({0})}})]
                    public partial class Target{{}}                        
                }}",
                        typeof(InterceptorWithRequirement).FullName.Replace("+", "."));
            }
        }


        [Test]
        public void MixinRequiredByInterceptorIsMixedIntoTarget()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            target.AddLogMessage("Test");

            Assert.True(target.Messages.Count > 0, "No evidence Mixin method AddLogMessage was called.");
        }

        [Test]
        public void InterceptorManipulatedMethodReturnValue()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");
            
            Assert.True(target.Method() == "Mixin", "Method() should return correct value");

            Assert.True(target.Messages.Count > 1, "No evidence Mixin method AddLogMessage was called.");

            Assert.True(target.Messages.Contains("OnBeforeMethod: Method"), "OnBeforeMethod event was not logged");

            Assert.True(target.Messages.Contains("OnAfterMethod: Method"), "OnAfterMethod event was not logged");
        }
    }
}
