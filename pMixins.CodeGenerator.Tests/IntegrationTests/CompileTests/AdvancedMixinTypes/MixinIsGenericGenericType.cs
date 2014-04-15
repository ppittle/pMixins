//----------------------------------------------------------------------- 
// <copyright file="MixinIsGenericGenericType.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    public class MixinIsGenericGenericType : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"                       
                        namespace Test
                        {
                            public interface IPrettyPrint{ string PrettyPrint(string s);}

                            public class TopGenericClass<T, K> where T : IPrettyPrint, new()
                            {
                                public string TopGenericStringMethod(string s, K a)
                                {
                                    return ""TopGeneric"" + a.ToString() + new T().PrettyPrint(s);
                                }
                            }

                            public class BottomGenericClass<T> : IPrettyPrint
                            {
                                public string PrettyPrint(string s)
                                {
                                    return ""BottomGeneric"" + s + typeof(T).Name;               
                                }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (TopGenericClass<BottomGenericClass<int>, bool>))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void PrimaryGenericMethodShouldBeInjected()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "TopGenericStringMethod",
                    ReflectionHelper.DefaultBindingFlags,
                    "Test", true)
                .ShouldEqual("TopGenericTrueBottomGenericTestInt32");
        }

        [Test]
        public void SecondaryGenericMethodShouldNotBeInjected()
        {
            var target = CompilerResults.TryLoadCompiledType("Test.Target");

            target.GetType().GetMethods().Any(x => x.Name == "PrettyPrint").ShouldBeFalse();
        }
    }
}
