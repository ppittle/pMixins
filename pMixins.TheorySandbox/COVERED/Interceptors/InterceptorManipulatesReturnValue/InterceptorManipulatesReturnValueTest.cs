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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Interceptors.InterceptorManipulatesReturnValue
{
    /// <summary>
    /// Covered in 
    ///     <see cref="ManipulateReturnValueInterceptorTest"/>
    /// </summary>
    public class InterceptorManipulatesReturnValueTest : SpecTestBase
    {
        protected InterceptorManipulatesReturnValueSpec _spec;

        protected override void Establish_context()
        {
            _spec = new InterceptorManipulatesReturnValueSpec();
        }

        /// <summary>
        /// Covered in 
        ///     <see cref="ManipulateReturnValueInterceptorTest.InterceptorManipulatedMethodReturnValue"/>
        /// </summary>
        [Test]
        public void Interceptor_Manipulate_Method_ReturnValue()
        {
            _spec.Method().ShouldEqual("Mixin_Interceptor");

            _spec.WasMethodCalled.ShouldEqual(true);
        }
    }
}
