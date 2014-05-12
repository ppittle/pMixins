//----------------------------------------------------------------------- 
// <copyright file="TargetCanAccessProtectedMembers.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ProtectedMembersTests
{
    public class TargetCanAccessProtectedMembers : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class ExampleMixin
                            {
                                protected string ProtectedDataMember;

                                protected string ProtectedProperty {get;set;}   

                                protected string ProtectedMethod()
                                {
                                    return ""Protected Method"";
                                }

                                public string Foo()
                                {
                                    return ""Foo"";
                                }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.ExampleMixin))]
                            public partial class Target
                            {
                                public string ElevatedProtectedMethod()
                                {
                                    return ProtectedMethod();
                                }

                                public string ElevatedProtectedProperty
                                {
                                    get { return ProtectedProperty; }
                                    set { ProtectedProperty = value; }
                                }
                            }                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallElevatedProtectedMethod()
        {
            CompilerResults
               .ExecuteMethod<string>(
                   "Test.Target",
                   "ElevatedProtectedMethod")
           .ShouldEqual("Protected Method");
        }

        [Test]
        public void CanCallGetSetOnElevatedProtectedProperty()
        {
            const string getSetStringTest = "SuperPropertyTest!!";

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            if (null == targetInstance)
                Assert.Fail("Failed to load Test.Target instance");

            ReflectionHelper.ExecutePropertySet(
                targetInstance,
                "ElevatedProtectedProperty",
                getSetStringTest);

            ReflectionHelper.ExecutePropertyGet<string>(
                targetInstance,
                "ElevatedProtectedProperty")
                .ShouldEqual(getSetStringTest);
        }
    }
}
