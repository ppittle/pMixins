//----------------------------------------------------------------------- 
// <copyright file="MixinVirtualMembersAreInjectedAsVirtual.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, January 31, 2014 10:45:52 AM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests
{
    [TestFixture]
    public class MixinVirtualMembersAreInjectedAsVirtual: GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithVirtualMembers
                            {
                                public virtual void VoidMethod()
                                {
                                    global::System.Console.WriteLine(""Void Method Called!"");
                                }

                                public virtual string StringMethod()
                                {
                                    return ""Mixin Method"";
                                }

                                public virtual string StringProperty { get; set; }      
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.MixinWithVirtualMembers))]
                            public partial class Target
                            {
                                public Target()
                                {
                                    ___mixins.Test_MixinWithVirtualMembers.StringMethodFunc = () => ""Overridden Method"";
                                }
                            }

                            public class Child : Target
                            {
                                public override string StringMethod()
                                {
                                    return ""Child Method"";
                                }
                            }                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallVirtualVoidMethod()
        {
            CompilerResults
                .ExecuteVoidMethod(
                    "Test.Target",
                    "VoidMethod");
        }

        [Test]
        public void CanCallVirtualMethodOverriddenInTarget()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "StringMethod")
                .ShouldEqual("Overridden Method");
        }

        [Test]
        public void CanCallVirtualMethodOverriddenInChild()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Child",
                    "StringMethod")
                .ShouldEqual("Child Method");
        }
    }
}
