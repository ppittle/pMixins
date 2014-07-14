//----------------------------------------------------------------------- 
// <copyright file="GenericInterfaceInheritance.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 11, 2014 1:57:16 PM</date> 
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
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests
{
    public class GenericInterfaceInheritance : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        using NUnit.Framework;
                        using CopaceticSoftware.pMixins.Attributes;

                        namespace Test
                        {
                            public class RandomTypeParam{}

                            //Interfaces

                            public interface IGrandparent<T>{}

                            public interface IParent<T> : IGrandparent<T> {}

                            public interface IChild<T> : IParent<T>{}

                            public interface ISimpleInterface<in T>{}
                            
                            public interface IDoubleInterface<T1,out T2> : IParent<T1>{}

                            public interface IComposite<T> : IParent<T>, ISimpleInterface<T>{}

                            public interface IMultiComposite<T1, T2, T3> : IDoubleInterface<T3,T2>,
                                                                           ISimpleInterface<T1>,
                                                                           IChild<T3>{}

                            public interface IHybrid<T> : IDoubleInterface<RandomTypeParam, T>{}

                            ///Mixins
                            

                            public class NonGenericMixin : IHybrid<int>, IComposite<string>{}

                            public class SimpleGenericMixin<T> : ISimpleInterface<T>,
                                                                 IChild<T>{}

                            public class HybridMixin<T> : IHybrid<T>{}

                            public class CompositeMixin<T> : IComposite<T>{}

                            public class MultiCompositeMixin<T1,T2, T3> : IMultiComposite<T1, T2, T3>{}

                            public class MultiCompositeLimitedMixin<T1,T2> : IMultiComposite<T1, RandomTypeParam, T2>{}


                            //Targets

                            [pMixin(Mixin = typeof (NonGenericMixin))]
                            public partial class NonGenericTarget{}

                            [pMixin(Mixin = typeof (SimpleGenericMixin<int>))]
                            public partial class SimpleGenericTarget{}

                            [pMixin(Mixin = typeof (HybridMixin<float>))]
                            public partial class HybridTarget{}

                            [pMixin(Mixin = typeof (CompositeMixin<string>))]
                            [pMixin(Mixin = typeof (CompositeMixin<bool>))]
                            public partial class CompositeTarget{}

                            [pMixin(Mixin = typeof (MultiCompositeMixin<int,RandomTypeParam, double>))]
                            public partial class MultiCompositeTarget{}   

                            [pMixin(Mixin = typeof (MultiCompositeLimitedMixin<int, double>))]
                            public partial class MultiCompositeLimitedTarget{}                           


                            public class TestHelper
                            {
                                public void NonGenericTargetTests()
                                {
                                    Assert.True(
                                        new NonGenericTarget() is IHybrid<int>,
                                        ""NonGeneric Target - is IHybrid<int>""); 

                                    Assert.True(
                                        new NonGenericTarget() is IDoubleInterface<RandomTypeParam, int>,
                                        ""NonGeneric Target - is IDoubleInterface<RandomTypeParam, int>""); 

                                    Assert.True(
                                        new NonGenericTarget() is IComposite<string>,
                                        ""NonGeneric Target - is IComposite<string>""); 

                                    Assert.True(
                                        new NonGenericTarget() is IParent<string>,
                                        ""NonGeneric Target - is IParent<string>""); 

                                    Assert.True(
                                        new NonGenericTarget() is IGrandparent<string>,
                                        ""NonGeneric Target - is IGrandparent<string>""); 

                                    Assert.True(
                                        new NonGenericTarget() is ISimpleInterface<string>,
                                        ""NonGeneric Target - is ISimpleInterface<string>""); 
                                }

                                public void SimpleGenericTargetTests()
                                {
                                   Assert.True(
                                        new SimpleGenericTarget() is IChild<int>,
                                        ""SimpleGeneric Target - is IChild<int>""); 

                                    Assert.True(
                                        new SimpleGenericTarget() is IParent<int>,
                                        ""SimpleGeneric Target - is IParent<int>""); 

                                    Assert.True(
                                        new SimpleGenericTarget() is IGrandparent<int>,
                                        ""SimpleGeneric Target - is IGrandparent<int>""); 

                                    Assert.True(
                                        new SimpleGenericTarget() is ISimpleInterface<int>,
                                        ""SimpleGeneric Target - is ISimpleInterface<int>""); 
                                }

                                public void HybridTargetTests()
                                {
                                   Assert.True(
                                        new HybridTarget() is IHybrid<float>,
                                        ""Hybrid Target - is IHybrid<float>""); 

                                    Assert.True(
                                        new HybridTarget() is IDoubleInterface<RandomTypeParam, float>,
                                        ""Hybrid Target - is IDoubleInterface<RandomTypeParam, float>"");                                     
                                }


                                public void CompositeTargetTests()
                                {
                                   Assert.True(
                                        new CompositeTarget() is IComposite<string>,
                                        ""Composite Target - is IComposite<string>""); 

                                    Assert.True(
                                        new CompositeTarget() is IParent<string>,
                                        ""Composite Target - is IParent<string>""); 

                                    Assert.True(
                                        new CompositeTarget() is IGrandparent<string>,
                                        ""Composite Target - is IGrandparent<string>""); 

                                    Assert.True(
                                        new CompositeTarget() is ISimpleInterface<string>,
                                        ""Composite Target - is ISimpleInterface<string>""); 

                                    Assert.True(
                                        new CompositeTarget() is IComposite<bool>,
                                        ""Composite Target - is IComposite<bool>""); 

                                    Assert.True(
                                        new CompositeTarget() is IParent<bool>,
                                        ""Composite Target - is IParent<bool>""); 

                                    Assert.True(
                                        new CompositeTarget() is IGrandparent<bool>,
                                        ""Composite Target - is IGrandparent<bool>""); 

                                    Assert.True(
                                        new CompositeTarget() is ISimpleInterface<string>,
                                        ""Composite Target - is ISimpleInterface<string>""); 
                                }

                                public void MultiCompositeTargetTests()
                                {
                                   Assert.True(
                                        new MultiCompositeTarget() is IMultiComposite<int, RandomTypeParam, double>,
                                        ""MultiComposite Target - is IMultiComposite<int, RandomTypeParam, double>""); 

                                    Assert.True(
                                        new MultiCompositeTarget() is IDoubleInterface<double, RandomTypeParam>,
                                        ""MultiComposite Target - is IDoubleInterface<double, RandomTypeParam>""); 

                                    Assert.True(
                                        new MultiCompositeTarget() is ISimpleInterface<int>,
                                        ""MultiComposite Target - is ISimpleInterface<int>""); 

                                    Assert.True(
                                        new MultiCompositeTarget() is IChild<double>,
                                        ""MultiComposite Target - is IChild<double>""); 

                                    Assert.True(
                                        new MultiCompositeTarget() is IParent<double>,
                                        ""MultiComposite Target - is IParent<double>""); 

                                    Assert.True(
                                        new MultiCompositeTarget() is IGrandparent<double>,
                                        ""MultiComposite Target - is IGrandparent<double>""); 
                                }

                                public void MultiCompositeLimitedTargetTests()
                                {
                                   Assert.True(
                                        new MultiCompositeLimitedTarget() is IMultiComposite<int, RandomTypeParam, double>,
                                        ""MultiCompositeLimited Target - is IMultiComposite<int, RandomTypeParam, double>""); 

                                    Assert.True(
                                        new MultiCompositeLimitedTarget() is IDoubleInterface<double, RandomTypeParam>,
                                        ""MultiCompositeLimited Target - is IDoubleInterface<double, RandomTypeParam>""); 

                                    Assert.True(
                                        new MultiCompositeLimitedTarget() is ISimpleInterface<int>,
                                        ""MultiCompositeLimited Target - is ISimpleInterface<int>""); 

                                    Assert.True(
                                        new MultiCompositeLimitedTarget() is IChild<double>,
                                        ""MultiCompositeLimited Target - is IChild<double>""); 

                                    Assert.True(
                                        new MultiCompositeLimitedTarget() is IParent<double>,
                                        ""MultiCompositeLimited Target - is IParent<double>""); 

                                    Assert.True(
                                        new MultiCompositeLimitedTarget() is IGrandparent<double>,
                                        ""MultiCompositeLimited Target - is IGrandparent<double>""); 
                                }
                            }      
                        }
                    ";
            }
        }

        [Test]
        public void NonGenericTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "NonGenericTargetTests");
        }

        [Test]
        public void SimpleGenericTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "SimpleGenericTargetTests");
        }

        [Test]
        public void HybridTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "HybridTargetTests");
        }

        [Test]
        public void CompositeTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "CompositeTargetTests");
        }

        [Test]
        public void MultiCompositeTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "MultiCompositeTargetTests");
        }

        [Test]
        public void MultiCompositeLimitedTargetTests()
        {
            CompilerResults.ExecuteVoidMethod(
                "Test.TestHelper",
                "MultiCompositeLimitedTargetTests");
        }
    }
}
