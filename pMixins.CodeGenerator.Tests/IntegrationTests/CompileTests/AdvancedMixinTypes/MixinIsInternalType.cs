//----------------------------------------------------------------------- 
// <copyright file="MixinIsInternalType.cs" company="Copacetic Software"> 
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

using System.Linq;
using System.Reflection;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    public class MixinIsInternalType : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class TestShim
                            {
                                public string CallTargetInternalMethod()
                                {
                                    return new Target().InternalMethod();       
                                }
                            }

                            internal class InternalClass
                            {
                                internal string InternalMethod(){ return ""Internal""; }                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.InternalClass))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void InternalMethodShouldBeInjected()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.TestShim",
                    "CallTargetInternalMethod")
                .ShouldEqual("Internal");
        }

        [Test]
        public void InternalMethodShouldHaveCorrectModifier()
        {
            var target = CompilerResults.TryLoadCompiledType("Test.Target");

            var method = target.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "InternalMethod");

            Assert.True(null != method, "Couldn't load method definition");

            method.IsPublic.ShouldBeFalse();
        }
    }
}
