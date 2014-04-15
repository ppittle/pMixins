//----------------------------------------------------------------------- 
// <copyright file="BasicInterceptorSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 21, 2014 12:24:08 AM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Interceptors.BasicInterceptor
{
    /// <summary>
    /// Covered in:
    ///     <see cref="BasicInterceptorTest"/>
    /// </summary>
    public class BasicInterceptorSpecTest : SpecTestBase
    {
        protected BasicInterceptorSpec _spec;

        protected override void Establish_context()
        {
            _spec = new BasicInterceptorSpec();

            _spec.Method(10).ShouldEqual("10");

            _spec.Property = "Test";

            _spec.Property.ShouldEqual("Test");

        }

        /// <summary>
        /// Covered in:
        ///     <see cref="BasicInterceptorTest.InterceptorInterceptedEvents"/>
        /// </summary>
        [Test]
        public void Interceptor_Intercepted_Events()
        {
            MixinInterceptor.InitializedCount.ShouldEqual(1);
            MixinInterceptor.BeforeMethodInvocationCount.ShouldEqual(1);
            MixinInterceptor.AfterMethodInvocationCount.ShouldEqual(1);
            MixinInterceptor.BeforePropertyGetInvocationCount.ShouldEqual(1);
            MixinInterceptor.AfterPropertyGetInvocationCount.ShouldEqual(1);
            MixinInterceptor.BeforePropertySetInvocationCount.ShouldEqual(1);
            MixinInterceptor.AfterPropertySetInvocationCount.ShouldEqual(1);
        }
        
    }
}
