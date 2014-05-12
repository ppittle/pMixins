//----------------------------------------------------------------------- 
// <copyright file="MixinStaticMembersAreInjectedIntoTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 5:12:23 PM</date> 
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


using System.Reflection;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests
{
    public class MixinStaticMembersAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class StaticMixin
                            {
                                public static string PublicPrettyPrint(string name)
                                   {
                                       return ""Public_"" + name;
                                   }

                                protected static string ProtectedPrettyPrint(string name)
                                {
                                    return ""Protected_"" + name;
                                }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.StaticMixin))]
                            public partial class Target
                            {
                                public static string ElevatedProtectedMethod(string name)
                                {
                                    return ProtectedPrettyPrint(name);
                                }
                            }

                            public class Child : Target
                            {
                                public static string ChildElevatedProtectedMethod(string name)
                                {
                                    return ProtectedPrettyPrint(name);
                                }
                            }                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallPublicMethodOnTarget()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicPrettyPrint",
                    BindingFlags.Static | BindingFlags.Public,
                    "42")
                .ShouldEqual("Public_42");
        }

        [Test]
        public void CanCallElevatedProtectedMethodOnTarget()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "ElevatedProtectedMethod",
                    BindingFlags.Static | BindingFlags.Public,
                    "42")
                .ShouldEqual("Protected_42");
        }

        [Test]
        public void CanCallElevatedProtectedMethodOnChild()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Child",
                    "ChildElevatedProtectedMethod",
                    BindingFlags.Static | BindingFlags.Public,
                    "42")
                .ShouldEqual("Protected_42");
        }
    }
}
