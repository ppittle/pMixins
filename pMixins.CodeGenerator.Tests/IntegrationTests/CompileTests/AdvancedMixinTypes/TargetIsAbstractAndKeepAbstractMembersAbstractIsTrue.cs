//----------------------------------------------------------------------- 
// <copyright file="TargetIsAbstractAndKeepAbstractMembersAbstractIsTrue.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, August 8, 2014 9:14:21 PM</date> 
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
    public class TargetIsAbstractAndKeepAbstractMembersAbstractIsTrue : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public abstract class Mixin
                            {
                                 public abstract int Number {get;}
                                    
                                 public int NumberPlusOne()
                                 {
                                      return Number + 1;
                                 }
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin),
                                KeepAbstractMembersAbstract = true)]                            
                            public partial class Target {}      

                            public class Child : Target{
                                public override int Number{ get{ return 42; }
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
    }
}
