//----------------------------------------------------------------------- 
// <copyright file="MixinIsGenericWithChildType.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, August 18, 2014 11:06:02 AM</date> 
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
    public class MixinIsGenericWithChildType : GenerateCodeAndCompileTestBase
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
                                public class Child
                                {
                                    public int Number = 42;
                                }

                                public GenericMixin<T>.Child GetChild()
                                {
                                    return new Child();
                                }                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (GenericMixin<int>))]                           
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void MethodThatReturnsChild()
        {
            dynamic target = CompilerResults
                .TryLoadCompiledType("Test.Target");

            ((int)target.GetChild().Number)
                .ShouldEqual(42);
        }
    }
}
