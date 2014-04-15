//----------------------------------------------------------------------- 
// <copyright file="BasicConceptSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 22, 2013 10:04:30 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.BasicConcept
{
    [TestFixture]
    public class BasicConceptSpecTest : SpecTestBase
    {
        private BasicConceptSpec _spec;

        protected override void Establish_context()
        {
            _spec = new BasicConceptSpec();
        }

        /// <summary>
        /// Covered in:
        ///  <see cref="MixinPublicMethodsAreInjectedIntoTarget"/>
        /// </summary>
        [Test]
        public void Can_Call_Mixin_Method()
        {
            _spec.Foo().ShouldEqual(new ExampleMixin().Foo());
        }

        /// <summary>
        /// Covered in:
        ///  <see cref="StaticImplicitConversionOperator"/>
        /// </summary>
        [Test]
        public void Can_Cast_Host_As_Mixin()
        {
            //Explicit cast
            var explicitCast = (ExampleMixin)_spec;
            explicitCast.ShouldNotBeNull();

            //Implicit cast
            ExampleMixin implicitCast = _spec;
            implicitCast.ShouldNotBeNull();
        }
    }
}
