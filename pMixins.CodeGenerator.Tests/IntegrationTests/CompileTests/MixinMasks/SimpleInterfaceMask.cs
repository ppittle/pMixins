//----------------------------------------------------------------------- 
// <copyright file="SimpleInterfaceMask.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 20, 2014 3:40:48 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.MixinMasks
{
    [TestFixture]
    public class SimpleInterfaceMask : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public interface IMixin
                            {
                                int InterfaceMethod();
                            }
                        

                            public class Mixin : IMixin
                            {
                                public int InterfaceMethod()
                                {
                                    return 42;
                                }

                                public int ClassMethod()
                                {
                                    return -1;   
                                }                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin), 
                                Masks = new System.Type[]{typeof(Test.IMixin)})]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallInterfaceMethod()
        {
            CompilerResults
               .ExecuteMethod<int>(
                   "Test.Target",
                   "InterfaceMethod")
               .ShouldEqual(42);
        }

        [Test]
        public void MethodExcludedByMaskShouldNotBePresent()
        {
            CompilerResults.TryLoadCompiledType("Test.Target")
                .GetType()
                .GetMethod("ClassMethod", BindingFlags.Instance | BindingFlags.Public)
                .ShouldBeNull();
        }

    }
}
