//----------------------------------------------------------------------- 
// <copyright file="MixinIsTargetsNestedClassSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 20, 2014 10:05:04 PM</date> 
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
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinIsTargetsNestedClass
{
    /// <summary>
    /// Covered by:
    ///     <see cref="MixinIsPrivateNestedTypeInTarget"/>
    /// </summary>
    [TestFixture]
    public class MixinIsTargetsNestedClassSpecTest : SpecTestBase
    {
        private MixinIsTargetsNestedClassSpec _spec;

        protected override void Establish_context()
        {
            _spec = new MixinIsTargetsNestedClassSpec();
        }

        /// <summary>
        /// Covered by:
        ///     <see cref="MixinIsPrivateNestedTypeInTarget.CanCallAllPublicMethods"/>
        /// </summary>
        [Test]
        public void Can_Call_All_Public_Members()
        {
            _spec.BaseMethod().ShouldEqual("Base Method");

            _spec.IInterfaceMethod().ShouldEqual("IInterfaceMethod");

            _spec.MixinMethod().ShouldEqual("Nested Private Mixin");
        }

        /// <summary>
        /// Covered by:
        ///     <see cref="MixinIsPrivateNestedTypeInTarget.CanCastTargetToBaseClass"/>
        /// </summary>
        [Test]
        public void Can_Cast_Spec_To_Base_Class()
        {
            BaseClass implicitCast = new MixinIsTargetsNestedClassSpec();
            implicitCast.ShouldNotBeNull();

            var explicitCast = (BaseClass) _spec;
            explicitCast.ShouldNotBeNull();
        }
    }
}
