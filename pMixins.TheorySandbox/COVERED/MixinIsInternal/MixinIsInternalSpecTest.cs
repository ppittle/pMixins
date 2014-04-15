//----------------------------------------------------------------------- 
// <copyright file="MixinIsInternalSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 20, 2014 3:03:43 PM</date> 
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

using System.Reflection;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinIsInternal
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinIsInternalType"/>
    /// </summary>
    [TestFixture]
    public class MixinIsInternalSpecTest : SpecTestBase
    {
        private MixinIsInternalSpec _spec;

        protected override void Establish_context()
        {
            _spec = new MixinIsInternalSpec();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinIsInternalType.InternalMethodShouldBeInjected"/>
        /// </summary>
        [Test]
        public void Internal_Method_Should_Be_Injected()
        {
            _spec.InternalMethod().ShouldEqual("Internal");
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinIsInternalType.InternalMethodShouldHaveCorrectModifier"/>
        /// </summary>
        [Test]
        public void Internal_Method_Should_Have_Correct_Modifier()
        {
            _spec.GetType().GetMember("InternalMethod", 
                BindingFlags.NonPublic | BindingFlags.Instance).ShouldNotBeEmpty();
        }
    }
}
