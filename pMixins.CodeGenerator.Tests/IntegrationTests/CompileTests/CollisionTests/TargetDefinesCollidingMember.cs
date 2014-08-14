//----------------------------------------------------------------------- 
// <copyright file="TargetDefinesCollidingMember.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, August 13, 2014 2:51:05 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.CollisionTests
{
    public class TargetDefinesCollidingMember : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class Mixin
                            {
                                public int Collision()
                                {
                                    return 100000;
                                }

                                 public int MixinMethod()
                                 {
                                      return 42;
                                 }
                            }
   
                            
                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Mixin))]
                            public partial class Target
                            {
                                public int Collision()
                                {
                                    return 24;
                                }
                            }                      
                        }
                    ";
            }

        }

        [Test]
        public void CanCallNonCollidingMixinMethod()
        {
            CompilerResults
                .ExecuteMethod<int>(
                    "Test.Target",
                    "MixinMethod")
                .ShouldEqual(42);
        }

        [Test]
        public void CanCallTargetMethod()
        {
            CompilerResults
                .ExecuteMethod<int>(
                    "Test.Target",
                    "Collision")
                .ShouldEqual(24);
        }
    }
}
