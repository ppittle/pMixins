//----------------------------------------------------------------------- 
// <copyright file="MixinIsAbstractWithProtectedNonParameterlessConstructorSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 2, 2014 3:10:46 PM</date> 
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
using CopaceticSoftware.pMixins.TheorySandbox.MixinIsAbstractWithProtectedNonParameterlessConstructor;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinIsAbstractWithProtectedNonParameterlessConstructor
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinIsAbstractWithProtectedNonParameterlessConstructor"/>
    /// </summary>
    [TestFixture]
    public class MixinIsAbstractWithProtectedNonParameterlessConstructorSpecTest : SpecTestBase
    {
        private class ChildClass : MixinIsAbstractWithProtectedNonParameterlessConstructorSpec
        {
            public bool CanAccessAllProtectedMembers()
            {
                var result =
                    ProtectedAbstractMethod() +
                    ProtectedMethod() +
                    ProtectedStaticMethod() +
                    ProtectedVirtualMethod(1);

                return true;
            }
            
            public override string PublicVirtualMethod()
            {
                return "ChildVirtualMethod";
            }
        }

        private MixinIsAbstractWithProtectedNonParameterlessConstructorSpec _spec;
        private ChildClass _childClass;
        protected override void Establish_context()
        {
            _spec = new MixinIsAbstractWithProtectedNonParameterlessConstructorSpec();

            
            _childClass = new ChildClass();
        }

        [Test]
        public void Can_Call_All_Public_Members()
        {
            _spec.PublicAbstractMethod().ShouldEqual("Target-PublicAbstractMethod");

            _spec.PublicVirtualMethod().ShouldEqual("Target's Public Virtual Method");

            _spec.RegularMethod().ShouldEqual("Hello World");

            MixinIsAbstractWithProtectedNonParameterlessConstructorSpec.PublicStaticMethod().ShouldEqual("public static method");

            _spec.OtherMixinMethod().ShouldEqual("Other Mixin Method");

            _spec.PublicVirtualProperty = "Test";
            _spec.PublicVirtualProperty.ShouldEqual("Test");
        }

        [Test]
        public void Child_Class_Can_Access_Protected_Members()
        {
            _childClass.CanAccessAllProtectedMembers().ShouldBeTrue();
        }

        [Test]
        public void Child_Class_Can_Override_Virtual_Member()
        {
            _childClass.PublicVirtualMethod().ShouldEqual("ChildVirtualMethod");
        }

    }
}
