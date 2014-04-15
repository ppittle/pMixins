//----------------------------------------------------------------------- 
// <copyright file="MixinIsAbstractType.cs" company="Copacetic Software"> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    [TestFixture]
    public class MixinIsAbstractType : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public abstract class AbstractMixin
                            {
                                public abstract string AbstractProperty{ get;set;}

                                public abstract string GetName();

                                public string PrettyPrintName()
                                {
                                    return ""Pretty "" + GetName();
                                }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.AbstractMixin))]
                            public partial class Target
                            {
                                 public string GetNameImplementation()
                                {
                                    return ""Hello World!"";
                                }

                                public string AbstractPropertyImplementation { get; set; }
                            }                      
                        }
                    ";
            }

        }

        [Test]
        public void CanCallAbstractMethod()
        {
            CompilerResults
               .ExecuteMethod<string>(
                   "Test.Target",
                   "GetName")
               .ShouldEqual("Hello World!"); 
        }

        [Test]
        public void CanCallConcreteMethod()
        {
            CompilerResults
               .ExecuteMethod<string>(
                   "Test.Target",
                   "PrettyPrintName")
               .ShouldEqual("Pretty Hello World!"); 
        }

        [Test]
        public void CanCallAbstractProperty()
        {
            const string getSetStringTest = "SuperPropertyTest!!";

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            if (null == targetInstance)
                Assert.Fail("Failed to load Test.Target instance");

            ReflectionHelper.ExecutePropertySet(
                targetInstance,
                "AbstractProperty",
                getSetStringTest);

            ReflectionHelper.ExecutePropertyGet<string>(
                targetInstance,
                "AbstractProperty")
                .ShouldEqual(getSetStringTest);
        }
    }
}
