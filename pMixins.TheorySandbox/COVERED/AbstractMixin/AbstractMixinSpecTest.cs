//----------------------------------------------------------------------- 
// <copyright file="AbstractMixinSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, September 3, 2013 6:38:14 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.AbstractMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinIsAbstractType"/>
    ///     <see cref="TargetWithAbstractMixin"/>
    /// </summary>
    [TestFixture]
    public class AbstractMixinSpecTest : SpecTestBase
    {
        private AbstractMixinSpec _spec;

        protected override void Establish_context()
        {
            _spec = new AbstractMixinSpec();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinIsAbstractType.CanCallAbstractMethod"/>
        /// </summary>
        [Test]
        public void Can_Call_Abstract_Method()
        {
            _spec.GetName().ShouldNotBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinIsAbstractType.CanCallConcreteMethod"/>
        /// </summary>
        [Test]
        public void Can_Call_Concrete_Method()
        {
            _spec.PrettyPrintName().ShouldNotBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="TargetWithAbstractMixin.CanConvertTargetToAbstractMixin"/>
        /// </summary>
        [Test]
        public void Can_Convert_To_Abstract_Base_Class()
        {
            AbstractMixin mixin = _spec;

            mixin.ShouldNotBeNull();

            mixin.GetName().ShouldNotBeEmpty();
        }
    }
}
