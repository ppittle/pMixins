//----------------------------------------------------------------------- 
// <copyright file="HostCanOverrideAndExposeVirtualMixinMembersSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 6:45:04 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostCanOverrideAndExposeVirtualMixinMembers
{
    /// <summary>
    /// Covered in:
    ///    <see cref="MixinVirtualMembersAreInjectedAsVirtual"/>
    /// </summary>
    public class HostCanOverrideAndExposeVirtualMixinMembersSpecTest : SpecTestBase
    {
        private HostCanOverrideAndExposeVirtualMixinMembersSpec _spec;

        protected override void Establish_context()
        {
            _spec = new HostCanOverrideAndExposeVirtualMixinMembersSpec();
        }

        [Test]
        public void Can_Call_Virtual_Method()
        {
            _spec.PrettyPrint("HelloWorld").ShouldNotBeEmpty();
        }

        [Test]
        public void Virtual_Method_Should_Be_Overridden_In_Host()
        {
            const string name = "Hello World!";

            MixinWithVirtualMember specAsMixin = _spec;


            specAsMixin.PrettyPrint(name).ShouldNotEqual(
                new MixinWithVirtualMember().PrettyPrint(name));
        }

        [Test]
        public void Virtual_Method_Should_Be_Overriden_In_Child()
        {
            const string name = "Hello World!";

            MixinWithVirtualMember specAsMixin = _spec;
            MixinWithVirtualMember childAsMixin = new ChildTest();

            var childPrettyPrint = childAsMixin.PrettyPrint(name);

            specAsMixin.PrettyPrint(name).ShouldNotEqual(
                childAsMixin.PrettyPrint(name));
        }
    }

    public class ChildTest : HostCanOverrideAndExposeVirtualMixinMembersSpec
    {
        public override string PrettyPrint(string name)
        {
            return "Child" + name;
        }
    }
}
