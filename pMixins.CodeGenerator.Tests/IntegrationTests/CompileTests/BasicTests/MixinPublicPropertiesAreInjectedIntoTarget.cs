//----------------------------------------------------------------------- 
// <copyright file="MixinPublicPropertiesAreInjectedIntoTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinPublicPropertiesAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithPublicProperty
                            {
                                public MixinWithPublicProperty(){
                                    PublicGetPrivateSet = ""PrivateSet"";
                                }

                                public string PublicGetPrivateSet{ get; private set;}

                                public string PublicPropertyGetOnly
                                {
                                    get{ return ""Public Property Get Only""; }
                                }

                                public string PublicPropertyGetSet{ get;set; }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinWithPublicProperty))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallGetOnlyProperty()
        {
            CompilerResults
                .ExecutePropertyGet<string>(
                    "Test.Target",
                    "PublicPropertyGetOnly")
                .ShouldEqual("Public Property Get Only");
        }

        [Test]
        public void CanCallGetSetProperty()
        {
            const string getSetStringTest = "SuperPropertyTest!!";

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            if (null == targetInstance)
                Assert.Fail("Failed to load Test.Target instance");

            ReflectionHelper.ExecutePropertySet(
                targetInstance,
                "PublicPropertyGetSet",
                getSetStringTest);

            ReflectionHelper.ExecutePropertyGet<string>(
                targetInstance,
                "PublicPropertyGetSet")
                .ShouldEqual(getSetStringTest);
        }

        [Test]
        public void CanCallPublicGetPrivateSetProperty()
        {
            CompilerResults
                .ExecutePropertyGet<string>(
                    "Test.Target",
                    "PublicGetPrivateSet")
                .ShouldEqual("PrivateSet");
        }
    }
}
