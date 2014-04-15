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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Interceptors.InterceptorCancellationToken
{
    /// <summary>
    /// Covered in 
    ///     <see cref="CancellationInterceptorTest"/>
    /// </summary>
    public class InterceptorCancellationSpecTest : SpecTestBase
    {
        protected InterceptorCancellationSpec _spec;

        protected override void Establish_context()
        {
            _spec = new InterceptorCancellationSpec();
        }

        /// <summary>
        /// Covered in 
        ///     <see cref="CancellationInterceptorTest.InterceptorCanceledMethodInvocation"/>
        /// </summary>
        [Test]
        public void Interceptor_Canceled_Method_Invocation()
        {
            _spec.Method().ShouldEqual("Interceptor");

            _spec.WasMethodCalled.ShouldEqual(false);
        }
    }
}
