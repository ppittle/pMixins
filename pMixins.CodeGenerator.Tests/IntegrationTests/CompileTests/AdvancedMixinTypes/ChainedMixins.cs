//----------------------------------------------------------------------- 
// <copyright file="ChainedMixins.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, August 8, 2014 8:50:13 PM</date> 
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
    [TestFixture]
    public class ChainedMixins : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public abstract class BaseMixin
                            {
                                 public abstract int Number {get;}
                                    
                                 public int NumberPlusOne()
                                 {
                                      return Number + 1;
                                 }
                            }

   
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.BaseMixin),
                                KeepAbstractMembersAbstract = true)]
                            public abstract partial class Mixin1
                            {
                                public abstract int Mixin1Number {get;}

                                 public int Mixin1Method()
                                 {
                                    return Mixin1Number;
                                 }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.BaseMixin),
                                KeepAbstractMembersAbstract = true)]
                            public abstract partial class Mixin2
                            {
                                 public abstract int Mixin2Number {get;}

                                 public int Mixin2Method()
                                 {
                                    return Mixin2Number;
                                 }
                            }


                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin1))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin2))]
                            public partial class Target
                            {
                                public int Number1Implementation { get { return 42; } }

                                public int Mixin1NumberImplementation { get { return 1; }}
                                public int Mixin2NumberImplementation { get { return 2; }}
                            }                      
                        }
                    ";
            }

        }

        [Test]
        public void CanCallBaseMixinMethod()
        {
            CompilerResults
                .ExecuteMethod<int>(
                    "Test.Target",
                    "NumberPlusOne")
                .ShouldEqual(43);
        }

        [Test]
        public void CanCallMixin1Method()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PrettyPrintName")
                .ShouldEqual("Pretty Hello World!");
        }

        [Test]
        public void CanCallMixin2Method()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PrettyPrintName")
                .ShouldEqual("Pretty Hello World!");
        }
    }
}
