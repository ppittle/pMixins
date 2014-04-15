//----------------------------------------------------------------------- 
// <copyright file="MixinsWithSameClassName.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, February 25, 2014 11:13:28 PM</date> 
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


using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.CollisionTests
{
    [TestFixture]
    public class MixinsWithSameClassName : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                using System;

                namespace A
                {
                    public class MixinWithSameName{
                        public string AMethod(){return ""AMethod"";}
                    }
                }

                namespace B
                {
                    public class MixinWithSameName{
                        public string BMethod(){return ""BMethod"";}
                    }
                }


                namespace Test
                {
                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (A.MixinWithSameName))]
                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (B.MixinWithSameName))]
                    public partial class Target{}                        
                }";
            }
        }

        [Test]
        public void CanCallAMethod()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "AMethod")
                .ShouldEqual("AMethod");
        }

        [Test]
        public void CanCallBMethod()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "BMethod")
                .ShouldEqual("BMethod");
        }
       
    }
}
