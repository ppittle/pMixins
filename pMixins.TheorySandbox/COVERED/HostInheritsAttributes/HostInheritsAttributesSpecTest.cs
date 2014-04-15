//----------------------------------------------------------------------- 
// <copyright file="HostInheritsAttributesSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, November 4, 2013 9:43:59 PM</date> 
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
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsAttributes
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinAttributesAreInjectedIntoTarget"/>
    /// </summary>
    [TestFixture]
    public class HostInheritsAttributesSpecTest : SpecTestBase
    {
        private HostInheritsAttributesSpec _spec;

        protected override void Establish_context()
        {
            _spec = new HostInheritsAttributesSpec();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinAttributesAreInjectedIntoTarget.ClassShouldHaveInheritedAttributes"/>
        /// </summary>
        [Test]
        public void Class_Has_Inherited_Attributes()
        {
            typeof (HostInheritsAttributesSpec)
                .GetCustomAttributes(typeof(InheritedAttribute), false)
                .ShouldNotBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinAttributesAreInjectedIntoTarget.MethodShouldHaveInheritedAttributes"/>
        /// </summary>
        [Test]
        public void Method_Has_Inherited_Attributes()
        {
            typeof(HostInheritsAttributesSpec)
                .GetMethod("Foo")
                .GetCustomAttributes(typeof(InheritedAttribute), false)
                .ShouldNotBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinAttributesAreInjectedIntoTarget.ClassShouldNotHaveNonInheritedAttributes"/>
        /// </summary>
        [Test]
        public void Class_Does_Not_Have_NonInherited_Attributes()
        {
            typeof(HostInheritsAttributesSpec)
                .GetCustomAttributes(typeof(NonInheritedAttribute), false)
                .ShouldBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinAttributesAreInjectedIntoTarget.MethodShouldNotHaveNonInheritedAttributes"/>
        /// </summary>
        [Test]
        public void Method_Does_Not_Have_NonInherited_Attributes()
        {
            typeof(HostInheritsAttributesSpec)
                .GetMethod("Foo")
                .GetCustomAttributes(typeof(NonInheritedAttribute), false)
                .ShouldBeEmpty();
        }
    }
}
