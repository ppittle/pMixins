//----------------------------------------------------------------------- 
// <copyright file="MixinWithProtectedConstructorAndParametersSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 5:04:38 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinWithProtectedConstructor
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinDoesNotHaveParameterlessConstructor"/>
    /// </summary>
    [TestFixture]
    public class MixinWithProtectedConstructorAndParametersSpecTest : SpecTestBase
    {
        private MixinWithProtectedConstructorAndParametersSpec _spec;

        protected override void Establish_context()
        {
            _spec = new MixinWithProtectedConstructorAndParametersSpec("hello world");
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinDoesNotHaveParameterlessConstructor.CanCallMixinMethods"/>
        /// </summary>
        [Test]
        public void Prove_Mixin_Is_Initialized()
        {
            _spec.PrettyPrintName().ShouldNotBeEmpty();
        }
    }
}
