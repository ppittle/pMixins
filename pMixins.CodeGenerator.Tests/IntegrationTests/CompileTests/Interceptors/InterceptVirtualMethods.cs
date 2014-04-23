//----------------------------------------------------------------------- 
// <copyright file="InterceptVirtualMethods.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 23, 2014 2:58:38 PM</date> 
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
using CopaceticSoftware.pMixins.Interceptors;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    [TestFixture]
    public class InterceptVirtualMethods : GenerateCodeAndCompileTestBase
    {
        #region MixinInterceptor
        public class MixinInterceptor : MixinInterceptorBase
        {
            public static int BeforeMethodInvocationCount = 0;
            
            public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
            {
                BeforeMethodInvocationCount++;
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
                        public virtual string Method(int i)
                        {{
                            return i.ToString();
                        }}
                    }}

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.Mixin),
                        Interceptors = new Type[] {{ typeof({0})}})]
                    public partial class Target{{}}                        
                }}",
                        typeof(MixinInterceptor).FullName.Replace("+", "."));
            }
        }

        public override void MainSetup()
        {
            base.MainSetup();

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            ReflectionHelper.ExecuteMethod<string>(
                targetInstance,
                "Method",
                ReflectionHelper.DefaultBindingFlags,
                10)
                .ShouldEqual("10");
        }

        [Test]
        public void InterceptorInterceptedEvents()
        {
            MixinInterceptor.BeforeMethodInvocationCount.ShouldEqual(1);
        }
    }
}
