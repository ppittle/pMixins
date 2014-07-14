//----------------------------------------------------------------------- 
// <copyright file="MixinDependencySpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, June 08, 2014 9:38:58 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.MixinDependency;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.ExpectedErrors;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinDependency
{
    /// <summary>
    /// Covered in:
    ///     <see cref="SimpleMixinDependency"/>
    ///     <see cref="MixinDependencyIsClass"/>
    ///     <see cref="MixinsShareDependency"/>
    ///     <see cref="MixinDependencyIsClassAndIsNotProvided"/>
    /// </summary>
    public class MixinDependencySpecTest : SpecTestBase
    {
        private MixinDependencySpec _spec;

        protected override void Establish_context()
        {
            _spec = new MixinDependencySpec();
        }

        [Test]
        public void OnDependencyShouldBeSet()
        {
            _spec.MixinMethod().ShouldEqual(42);

            _spec.OnDependencySetCalled.ShouldBeTrue();
        }
    }
}
