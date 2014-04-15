//----------------------------------------------------------------------- 
// <copyright file="MixinPublicMethodsAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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
    public class MixinPublicMethodsAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithPublicMethod
                            {
                                public void VoidMethod()
                                {
                                    global::System.Console.WriteLine(""Void Method Called!"");
                                }

                                public string PublicMethod()
                                {
                                    return ""Public Method"";
                                }

                                public string PublicMethodWithOneParameter(string p)
                                {
                                    return p;
                                }

                                public string PublicMethodWithTwoParameters(string p1, string p2)
                                {
                                    return p1 + p2;
                                }                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinWithPublicMethod))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallPublicMethod()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethod")
                .ShouldEqual("Public Method");
        }

        [Test]
        public void CanCallVoidMethod()
        {
            CompilerResults
                .ExecuteVoidMethod(
                    "Test.Target",
                    "VoidMethod");
        }

        [Test]
        public void CanCallMethodWithOneParameter()
        {
            const string p = "Hello";

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethodWithOneParameter",
                    ReflectionHelper.DefaultBindingFlags,
                    p)
                .ShouldEqual(p);
        }

        [Test]
        public void CanCallMethodWithTwoParameters()
        {
            const string p1 = "Hello";

            const string p2 = "Hello";

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethodWithTwoParameters",
                    ReflectionHelper.DefaultBindingFlags,
                    p1, p2)
                .ShouldEqual(p1 + p2);
        }
    }
}
