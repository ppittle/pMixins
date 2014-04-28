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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using CopaceticSoftware.pMixins.Interceptors;
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
                if (null != eventArgs.MemberInvocationException)
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
                    }}

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.Mixin),
                        Interceptors = new Type[] {{ typeof({0})}})]
                    public partial class Target{{}}                        
                }}",
                typeof(SuppressExceptionInterceptor).FullName.Replace("+", "."));
            }
        }

        [Test]
        public void InterceptorSuppressesMethodException()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            Assert.True(target.Method() == "Intercepted", "Exception in method should be suppressed");

            Assert.True(target.Property == "Intercepted", "Exception in property should be suppressed");
        }
    }
}
