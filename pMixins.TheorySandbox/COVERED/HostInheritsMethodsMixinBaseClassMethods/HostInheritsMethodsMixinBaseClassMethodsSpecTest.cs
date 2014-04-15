//----------------------------------------------------------------------- 
// <copyright file="HostInheritsMethodsMixinBaseClassMethodsSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 5:18:09 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsMethodsMixinBaseClassMethods
{
    /// <summary>
    /// Covered in
    ///     <see cref="MixinBaseClassMembersAreInjectedIntoTarget"/>
    /// </summary>
    [TestFixture]
    public class HostInheritsMethodsMixinBaseClassMethodsSpecTest : SpecTestBase
    {
        private HostInheritsMethodsMixinBaseClassMethodsSpec _spec;

        protected override void Establish_context()
        {
            _spec = new HostInheritsMethodsMixinBaseClassMethodsSpec();
        }

        /// <summary>
        /// Covered in
        ///     <see cref="MixinBaseClassMembersAreInjectedIntoTarget.CanCallBaseClassMethod"/>
        /// </summary>
        [Test]
        public void Can_Call_Mixin_Base_Class_Method()
        {
            const string sampleInput = "HelloWorld";

            _spec.PrettyPrint(sampleInput)
                .ShouldEqual(new ExampleMixin().PrettyPrint(sampleInput));
        }
    }
}
