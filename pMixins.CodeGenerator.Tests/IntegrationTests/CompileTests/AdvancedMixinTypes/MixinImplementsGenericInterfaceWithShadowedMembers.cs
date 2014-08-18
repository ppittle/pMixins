//----------------------------------------------------------------------- 
// <copyright file="MixinImplementsGenericInterfaceWithShadowedMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, August 18, 2014 1:23:22 PM</date> 
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
    public class MixinImplementsGenericInterfaceWithShadowedMembers : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public interface IBaseMixinInterface
                            {
                                void DoStuff();
                            }


                            public interface IMixin : IBaseMixinInterface
                            {
                                object GetThing(object o);

                                bool Method();
                            }

                            public interface IMixin<T> : IMixin
                            {
                                new T GetThing(T o);

                                new bool Method();
                            }

                            public class GenericMixin<T> : IMixin<T>
                            {
                                public T GetThing(T o)
                                {
                                    return o;
                                }            

                                public bool Method()
                                {
                                    return true;
                                }

                                object IMixin.GetThing(object o){
                                    return GetThing((T)o);
                                }

                                bool IMixin.Method()
                                {
                                    return false;
                                } 

                                public void DoStuff(){}                                        
                            }
                          
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (GenericMixin<int>))]                            
                            public partial class Target{}   

                            public class TestHarness
                            {
                                public bool CanCallShadowedMethod()
                                {
                                    return 
                                        (5 == 
                                            new Target().GetThing(5)) &&
                                        
                                        new Target().Method();

                                    
                                }

                                public bool CanCallBaseMethod()
                                {
                                    IMixin explicitCast = (IMixin)
                                        new Target();

                                    return 
                                        (5 == 
                                        (int)
                                        explicitCast.GetThing(5)) &&
                                        (!explicitCast.Method());
                                }
                            }                     
                        }
                    ";
            }
        }

        [Test]
        public void CanCallShadowedMethod()
        {
            CompilerResults
                .ExecuteMethod<bool>(
                    "Test.TestHarness",
                    "CanCallShadowedMethod")
                .ShouldEqual(true);
            
        }

        [Test]
        public void CanCallBaseMethod()
        {
            CompilerResults
                .ExecuteMethod<bool>(
                    "Test.TestHarness",
                    "CanCallBaseMethod")
                .ShouldEqual(true);
        }
    }
}
