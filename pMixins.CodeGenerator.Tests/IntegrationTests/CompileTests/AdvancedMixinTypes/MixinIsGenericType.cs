//----------------------------------------------------------------------- 
// <copyright file="MixinIsGenericType.cs" company="Copacetic Software"> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    public class MixinIsGenericType : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class GenericMixin<T>
                            {
                                public string PrettyPrint(T obj)
                                {
                                    return ""Pretty"" + obj.ToString();
                                }                                
                            }

                            public class GenericMixinTwoParams<T1,T2>
                            {
                                public string Concat(T1 a, T2 b)
                                {
                                    return a.ToString() + b.ToString();
                                }                                
                            }


                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (GenericMixin<int>))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (GenericMixin<bool>))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (GenericMixinTwoParams<int, string>))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void MethodWithClassLevelGenericParameterShouldBeInjected()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PrettyPrint",
                    ReflectionHelper.DefaultBindingFlags,
                    5)
                .ShouldEqual("Pretty5");

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PrettyPrint",
                    ReflectionHelper.DefaultBindingFlags,
                    true)
                .ShouldEqual("PrettyTrue");

            CompilerResults
               .ExecuteMethod<string>(
                   "Test.Target",
                   "Concat",
                   ReflectionHelper.DefaultBindingFlags,
                   42,"world")
               .ShouldEqual("42world");
        }
    }
}
