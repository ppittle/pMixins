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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Interceptors.InterceptorRequiresMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="InterceptorWithMixinRequirementTest"/>
    /// </summary>
    public class InterceptorRequiresMixinTest : SpecTestBase
    {
        protected InterceptorRequiresMixinSpec _spec;

        protected override void Establish_context()
        {
            _spec = new InterceptorRequiresMixinSpec();
        }

        [Test]
        public void Intercepts_Required_Mixin_Is_In_Target()
        {
            _spec.AddLogMessage("Test");

            _spec.Messages.Count.ShouldBeGreaterThan(1);
        }

        [Test]
        public void Interceptor_Intercepts_Method()
        {
            _spec.Method().ShouldEqual("Mixin");

            _spec.Messages.Count.ShouldBeGreaterThan(1);

            _spec.Messages.ShouldContain("OnBeforeMethod: Method");

            _spec.Messages.ShouldContain("OnAfterMethod: Method");
        }
    }
}
