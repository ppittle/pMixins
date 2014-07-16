//----------------------------------------------------------------------- 
// <copyright file="MixinImplementsAbstractType.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 16, 2014 2:21:07 PM</date> 
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

using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    /// <summary>
    /// https://github.com/ppittle/pMixins/issues/21
    /// </summary>
    [TestFixture]
    public class MixinImplementsAbstractType : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public abstract class AbstractBase
                            {
                                public abstract int GetNumber();

                                public virtual int VirtualMethod(){return 3;}
                            }

                            public class Mixin : AbstractBase
                            {
                                public override int GetNumber()                        
                                {
                                    return 42;
                                }
                                
                                public override int VirtualMethod(){
                                    return 24;
                                }
                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin))]
                            public partial class Target
                            {
                                 //Target should NOT need to provide an implementation for GetNumber
                            } 

                            public class TargetChild : Target
                            {
                                public override int GetNumber()                        
                                {
                                    return 1;
                                }
                            }                     
                        }
                    ";
            }
        }

        [Test]
        public void CanCallGetNumber()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            int number = target.GetNumber();

            number.ShouldEqual(42);
        }

        [Test]
        public void CanCallGetNumberOnChild()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.TargetChild");

            int number = target.GetNumber();

            number.ShouldEqual(1);
        }
    }

    public class CustomObject
    {
        public string Name { get; set; }
    }

    public class CustomObjectEqualityComparer : IEqualityComparer<CustomObject>
    {
        public bool Equals(CustomObject x, CustomObject y)
        {
            return true;
        }

        public int GetHashCode(CustomObject obj)
        {
            return 42;
        }
    }

    [TestFixture]
    public class TestRunner
    {
        private CustomObject[] customObjects = 
        {
            new CustomObject {Name = "Please"},
            new CustomObject {Name = "Help"},
            new CustomObject {Name = "Me"},
            new CustomObject {Name = "Stack"},
            new CustomObject {Name = "Overflow"},
        };

        [Test]
        public void DistinctTest()
        {
            var distinctArray =
                customObjects.Distinct(new CustomObjectEqualityComparer()).ToArray();

            Assert.AreEqual(1, distinctArray.Length);
        }
    }
}
