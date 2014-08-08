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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.CustomPMixinAttributes
{
    [TestFixture]
    public class BasicCustomPMixinAttribute : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            using CopaceticSoftware.pMixins.Attributes;

                            public class Mixin
                            {
                                public int Number{ get{ return 42;} }
                            }

                            public class CustomPMixinsAttribute : pMixinAttribute
                            {
                                public CustomPMixinsAttribute()
                                {
                                    Mixin = typeof(Mixin);
                                }
                            }

                            [CustomPMixins]                           
                            public partial class Target
                            {                                
                            }                      
                        }
                    ";
            }

        }

        [Test]
        public void CanCallMixinProperty()
        {
            CompilerResults
                .ExecutePropertyGet<int>(
                    "Test.Target",
                    "Number")
                .ShouldEqual(42);
        }
    }
}
