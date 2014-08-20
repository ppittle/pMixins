//----------------------------------------------------------------------- 
// <copyright file="NormalPartialClassWithNoMixinAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, August 20, 2014 3:40:55 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    /// <summary>
    /// Make sure pMixins doesn't interfare with code that doesn't use the framework.
    /// </summary>
    [TestFixture]
    public class NormalPartialClassWithNoMixinAttribute : GenerateCodeAndCompileTestBase
    {
        protected override bool FailIfNoCodeGenerated
        {
            get { return false; }
        }

        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public partial class NormalClass
                            {
                                public int Number{ get{ return 42;} }
                            }

                            public partial class NormalClass
                            {
                                public int OtherNumber{ get{ return 24;} }
                            }
                        }
                    ";
            }

        }

        [Test]
        public void CanCallNormalClassProperties()
        {
            CompilerResults
                .ExecutePropertyGet<int>(
                    "Test.NormalClass",
                    "Number")
                .ShouldEqual(42);

            CompilerResults
                .ExecutePropertyGet<int>(
                    "Test.NormalClass",
                    "OtherNumber")
                .ShouldEqual(24);
        }
    }
}
