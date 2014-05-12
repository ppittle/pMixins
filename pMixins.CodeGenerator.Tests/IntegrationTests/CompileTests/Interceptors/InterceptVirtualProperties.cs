//----------------------------------------------------------------------- 
// <copyright file="InterceptVirtualProperties.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 23, 2014 9:42:50 PM</date> 
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
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    [TestFixture]
    public class InterceptVirtualProperties : GenerateCodeAndCompileTestBase
    {
        #region MixinInterceptor
        public class MixinInterceptor : MixinInterceptorBase
        {
            public static int BeforePropertyGetInvocationCount = 0;
            public static int AfterPropertyGetInvocationCount = 0;
            public static int BeforePropertySetInvocationCount = 0;
            public static int AfterPropertySetInvocationCount = 0;
            
            public override void OnBeforePropertyInvocation(object sender, PropertyEventArgs eventArgs)
            {
                if (eventArgs.IsGet)
                    BeforePropertyGetInvocationCount++;
                else
                    BeforePropertySetInvocationCount++;
            }

            public override void OnAfterPropertyInvocation(object sender, PropertyEventArgs eventArgs)
            {
                if (eventArgs.IsGet)
                    AfterPropertyGetInvocationCount++;
                else
                    AfterPropertySetInvocationCount++;
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
                        public virtual string Property {{ get; set; }}
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

            ReflectionHelper.ExecutePropertySet(
                targetInstance,
                "Property",
                "Test");

            ReflectionHelper.ExecutePropertyGet<string>(
                targetInstance,
                "Property")
                .ShouldEqual("Test");
        }

        [Test]
        public void InterceptorInterceptedEvents()
        {
            MixinInterceptor.BeforePropertyGetInvocationCount.ShouldEqual(1);
            MixinInterceptor.AfterPropertyGetInvocationCount.ShouldEqual(1);
            MixinInterceptor.BeforePropertySetInvocationCount.ShouldEqual(1);
            MixinInterceptor.AfterPropertySetInvocationCount.ShouldEqual(1);
        }
    }
}
