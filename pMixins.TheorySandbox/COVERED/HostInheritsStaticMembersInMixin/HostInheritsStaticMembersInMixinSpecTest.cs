//----------------------------------------------------------------------- 
// <copyright file="HostInheritsStaticMembersInMixinSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, October 14, 2013 12:26:16 AM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsStaticMembersInMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinStaticMembersAreInjectedIntoTarget"/>
    /// </summary>
    [TestFixture]
    public class HostInheritsStaticMembersInMixinSpecTest : SpecTestBase
    {
        private class HostInheritsStaticMembersInMixinSpecInheritanceTest : HostInheritsStaticMembersInMixinSpec
        {
            public static string WrapperForHostProtectedPrettyPrint(string name)
            {
                return ProtectedPrettyPrint(name);
            }
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinStaticMembersAreInjectedIntoTarget.CanCallPublicMethodOnTarget"/>
        /// </summary>
        [Test]
        public void Can_Call_Host_Method_That_Uses_Public_Static_Mixin_Method()
        {
            HostInheritsStaticMembersInMixinSpec.PublicPrettyPrint("Hello World")
                .ShouldNotBeEmpty();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinStaticMembersAreInjectedIntoTarget.CanCallElevatedProtectedMethodOnTarget"/>
        /// </summary>
        [Test]
        public void Can_Call_Host_Method_That_Uses_Protected_Static_Mixin_Method()
        {
            HostInheritsStaticMembersInMixinSpec.WrapperForBaseProtectedPrettyPrint("Hello World")
                .ShouldNotBeEmpty();
        }


        /// <summary>
        /// Covered in:
        ///     <see cref="MixinStaticMembersAreInjectedIntoTarget.CanCallElevatedProtectedMethodOnChild"/>
        /// </summary>
        [Test]
        public void Child_Class_Can_Call_Hosts_Mixed_In_Protected_Static_Method()
        {

            HostInheritsStaticMembersInMixinSpecInheritanceTest.WrapperForHostProtectedPrettyPrint("Hello World")
                .ShouldNotBeEmpty();

            HostInheritsStaticMembersInMixinSpecInheritanceTest.WrapperForHostProtectedPrettyPrint("Hello World")
                .ShouldEqual(
                    HostInheritsStaticMembersInMixinSpec.WrapperForBaseProtectedPrettyPrint("Hello World"));
            
        }
    }
}
