//----------------------------------------------------------------------- 
// <copyright file="MultipleMixinsAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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
    public class MultipleMixinsAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class Mixin1
                            {
                                public void VoidMethod()
                                {
                                    global::System.Console.WriteLine(""Void Method Called!"");
                                }
                            }
    
                            public class Mixin2
                            {
                                public string PublicMethod()
                                {
                                    return ""Public Method"";
                                }
                            }

                            public class Mixin3
                            {
                                public string PublicMethodWithOneParameter(string p)
                                {
                                    return p;
                                }
                            }

                            public class Mixin4
                            {
                                public string PublicMethodWithTwoParameters(string p1, string p2)
                                {
                                    return p1 + p2;
                                }                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.Mixin1))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.Mixin2))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.Mixin3))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.Mixin4))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallAllTheMethods()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethod")
                .ShouldEqual("Public Method");
       
            CompilerResults
                .ExecuteVoidMethod(
                    "Test.Target",
                    "VoidMethod");
        
            const string p = "Hello";

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethodWithOneParameter",
                    ReflectionHelper.DefaultBindingFlags,
                    p)
                .ShouldEqual(p);
       
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
