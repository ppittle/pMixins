//----------------------------------------------------------------------- 
// <copyright file="InterceptorSuppressesException.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, April 28, 2014 11:40:35 PM</date> 
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
using CopaceticSoftware.pMixins.Interceptors;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    public class InterceptorSuppressesException : GenerateCodeAndCompileTestBase
    {
        #region MixinInterceptor

        public class SuppressExceptionInterceptor : MixinInterceptorBase
        {
            public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
            {
                if (null != eventArgs.MemberInvocationException && eventArgs.MemberName == "Method")
                    eventArgs.CancellationToken = new CancellationToken
                    {
                        Cancel = true,
                        ReturnValue = "Intercepted"
                    };
            }

            public override void OnAfterPropertyInvocation(object sender, PropertyEventArgs eventArgs)
            {
                if (null != eventArgs.MemberInvocationException)
                    eventArgs.CancellationToken = new CancellationToken
                    {
                        Cancel = true,
                        ReturnValue = "Intercepted"
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
                            throw new Exception(""Should be Suppressed"");
                        }}             

                        public string Property  
                        {{ get{{ throw new Exception(""Should be Suppressed""); }} }}

                        public void ThrowException()
                        {{
                            throw new Exception(""Should NOT be Suppressed"");
                        }}   

                    }}

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.Mixin),
                        Interceptors = new Type[] {{ typeof({0})}})]
                    public partial class Target{{}}                        
                }}",
                        typeof (SuppressExceptionInterceptor).FullName.Replace("+", "."));
            }
        }

        private dynamic target;

        public override void MainSetup()
        {
            base.MainSetup();

            target = CompilerResults.TryLoadCompiledType("Test.Target");

            Assert.True(null != target, "Failed to load Target");
        }

        [Test]
        public void InterceptorSuppressesMethodException()
        {
            Assert.True(target.Method() == "Intercepted", "Exception in method should be suppressed");
        }

        [Test]
        public void InterceptorSuppressesPropertyException()
        {
            Assert.True(target.Property == "Intercepted", "Exception in property should be suppressed");
        }

        [Test]
        public void InterceptorDoesNotSuppressesExceptionWhenItShouldNot()
        {
            try
            {
                target.ThrowException();

                Assert.Fail("Exception was suppressed when should not be.");
            }
            catch (Exception e)
            {
                Assert.True(e.Message.Equals("Should NOT be Suppressed"), "Incorrect exception was thrown.");
            }
        }
    }
}
