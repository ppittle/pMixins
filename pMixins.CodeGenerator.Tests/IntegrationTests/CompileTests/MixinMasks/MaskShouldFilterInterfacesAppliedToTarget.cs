//----------------------------------------------------------------------- 
// <copyright file="MaskShouldFilterInterfacesAppliedToTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 17, 2014 12:05:01 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.MixinMasks
{
    /// <summary>
    /// Test for https://github.com/ppittle/pMixins/issues/23.
    /// </summary>
    [TestFixture]
    public class MaskShouldFilterInterfacesAppliedToTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public interface IMixin1
                            {
                                int InterfaceMethod1();
                            }

                            public interface IMixin2
                            {
                                int InterfaceMethod2();
                            }
                        

                            public class Mixin : IMixin1, IMixin2
                            {
                                public int InterfaceMethod1()
                                {
                                    return 42;
                                }

                                public int InterfaceMethod2()
                                {
                                    return 24;
                                }                                                         
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin), 
                                Masks = new System.Type[]{typeof(Test.IMixin1)})]
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
                    "InterfaceMethod1")
                .ShouldEqual(42);
        }

        [Test]
        public void TargetShouldNotInheritIMixin2()
        {
            CompilerResults.GetMember(
                "Test.Target",
                new object[] { },
                "InterfaceMethod1")
                .ShouldNotBeNull();

            CompilerResults.GetMember(
                "Test.Target",
                new object[]{},
                "InterfaceMethod2")
                .ShouldBeNull();
        }
    }
}
