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

using System;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsMixinInterfaces
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinInterfacesAreInjectedIntoTarget"/>
    /// </summary>
    [TestFixture]
    public class HostInheritsMixinInterfacesSpecTest : SpecTestBase
    {
        private HostInheritsMixinInterfacesSpec _spec;

        protected override void Establish_context()
        {
            _spec = new HostInheritsMixinInterfacesSpec();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinInterfacesAreInjectedIntoTarget.CanCallInterfaceMethod"/>
        /// </summary>
        [Test]
        public void Can_Call_Interface_Method()
        {
            var sampleInput = DateTime.Now;

            _spec.ChildMethod(sampleInput)
                .ShouldEqual(new ExampleMixin().ChildMethod(sampleInput));
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinInterfacesAreInjectedIntoTarget.CanConvertTargetToMixinInterface"/>
        /// </summary>
        [Test]
        public void Can_Cast_Host_As_Mixin_Interface()
        {
            //Explicit cast
            var explicitCast = (IMixinParent)_spec;
            explicitCast.ShouldNotBeNull();

            //Implicit cast
            IMixinOther implicitCast = _spec;
            implicitCast.ShouldNotBeNull();
        }
    }
}
