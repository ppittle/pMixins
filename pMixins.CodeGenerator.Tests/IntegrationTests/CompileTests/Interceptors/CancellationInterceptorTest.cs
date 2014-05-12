//----------------------------------------------------------------------- 
// <copyright file="BasicInterceptorTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, February 26, 2014 7:31:56 PM</date> 
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

using CopaceticSoftware.pMixins.Interceptors;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    [TestFixture]
    public class CancellationInterceptorTest : GenerateCodeAndCompileTestBase
    {
        #region MixinInterceptor
        public class CancelInterceptor : MixinInterceptorBase
        {
            public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
            {
                eventArgs.CancellationToken = new CancellationToken
                {
                    Cancel = true,
                    ReturnValue = "Interceptor"
                };
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
                typeof(CancelInterceptor).FullName.Replace("+", "."));
            }
        }
     

        [Test]
        public void InterceptorCanceledMethodInvocation()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            Assert.True(target.Method() == "Interceptor", "Method() should be intercepted");

            Assert.True(target.WasMethodCalled == false, "WasMethodCalled should be false");
        }
    }
}
