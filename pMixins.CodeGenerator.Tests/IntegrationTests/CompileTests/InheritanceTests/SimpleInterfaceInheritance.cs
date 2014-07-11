//----------------------------------------------------------------------- 
// <copyright file="SimpleInterfaceInheritance.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 11, 2014 1:01:49 PM</date> 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests
{
    public class SimpleInterfaceInheritance : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        using NUnit.Framework;

                        namespace Test
                        {
                            public interface IGrandparent{}

                            public interface IParent : IGrandparent {}

                            public interface IChild : IParent{}

                            public interface ISimpleInterface{}

                            public interface IComposite : IParent, ISimpleInterface{}

                            public class CompositeMixin : IComposite{}

                            public class ComplexMixin : IChild, ISimpleInterface{}

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (CompositeMixin))]
                            public partial class CompositeTarget{}

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (ComplexMixin))]
                            public partial class ComplexTarget{}



                            public class TestHelper
                            {
                                public void CompositeTargetTests()
                                {
                                   Assert.True(
                                        new CompositeTarget() is IComposite,
                                        ""Composite Target - is IComposite""); 

                                    Assert.True(
                                        new CompositeTarget() is IParent,
                                        ""Composite Target - is IParent""); 

                                    Assert.True(
                                        new CompositeTarget() is IGrandparent,
                                        ""Composite Target - is IGrandparent""); 

                                    Assert.True(
                                        new CompositeTarget() is ISimpleInterface,
                                        ""Composite Target - is ISimpleInterface""); 
                                }

                                public void ComplexTargetTests()
                                {
                                   Assert.True(
                                        new ComplexTarget() is IChild,
                                        ""Complex Target - is IChild""); 

                                    Assert.True(
                                        new ComplexTarget() is IParent,
                                        ""Complex Target - is IParent""); 

                                    Assert.True(
                                        new ComplexTarget() is IGrandparent,
                                        ""Complex Target - is IGrandparent""); 

                                    Assert.True(
                                        new ComplexTarget() is ISimpleInterface,
                                        ""Complex Target - is ISimpleInterface""); 
                                }
                            } 
                
                                   
                        }
                    ";
            }
        }

        [Test]
        public void CompositeTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "CompositeTargetTests");
        }

        [Test]
        public void ComplexTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "ComplexTargetTests");
        }
    }
}
