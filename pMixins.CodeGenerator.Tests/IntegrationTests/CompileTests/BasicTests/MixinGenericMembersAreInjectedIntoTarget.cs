//----------------------------------------------------------------------- 
// <copyright file="MixinGenericMembersAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Generic;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinGenericMembersAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithGenericMethods
                            {
                                public void VoidMethod<T>(T p)
                                {
                                    global::System.Console.WriteLine(""Void Method Called: "" + p);
                                }

                                public string Method<T>()
                                {
                                    return ""Public Method"";
                                }

                                public T PublicMethodWithOneParameter<T>(object p)
                                {
                                    return (T)p;
                                }

                                public string PublicMethodWithTwoParameters<T,K>(T p1, K p2)
                                {
                                    return p1.ToString() + p2.ToString();
                                }                                
                            
                                public string PublicMethodWithGenericConstraint<T>(T a)
                                    where T : System.Exception                               
                                {
                                    return a.Message;
                                }

                                protected string ProtectedMethod<T>(T p)
                                {
                                    return p.ToString() + ""Protected"";
                                }     

                                public System.Collections.Generic.List<int> GenericProperty { get; set; }                            
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinWithGenericMethods))]
                            public partial class Target{}   

                            public class Child : Target
                            {
                                public string ProtectedMethod<T>(T p)
                                {
                                    return base.ProtectedMethod<T>(p);
                                }
                            }                     
                        }
                    ";
            }
        }

        [Test]
        public void CanCallVoidMethod()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            target.VoidMethod<string>("Hello World");
        }

        [Test]
        public void CanCallPublicMethod()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            string result = target.Method<int>();

            result.ShouldEqual("Public Method");
        }

        [Test]
        public void CanCallMethodWithOneParameter()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            int result = target.PublicMethodWithOneParameter<int>(10);

            result.ShouldEqual(10);
        }

        [Test]
        public void CanCallMethodWithTwoParameters()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            string result = target.PublicMethodWithTwoParameters<string, int>("Hello", 42);

            result.ShouldEqual("Hello42");
        }

        [Test]
        public void CanCallMethodWithGenericConstraint()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            var e = new NullReferenceException("Message");

            string result = target.PublicMethodWithGenericConstraint<NullReferenceException>(e);

            result.ShouldEqual(e.Message);
        }

        [Test]
        public void CanCallProtectedMethod()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Child");
            
            string result = target.ProtectedMethod<int>(42);

            result.ShouldEqual("42Protected");
        }

        [Test]
        public void CanCallProperty()
        {
            var testList = new List<int>{42};

            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            target.GenericProperty = testList;

            Assert.True(target.GenericProperty.Contains(42), "Property get/set failed.");


        }
    }
}
