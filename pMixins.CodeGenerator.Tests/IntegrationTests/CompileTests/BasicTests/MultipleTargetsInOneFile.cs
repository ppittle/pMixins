//----------------------------------------------------------------------- 
// <copyright file="MultipleTargetsInOneFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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
    [TestFixture]
    public class MultipleTargetsInOneFile : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                    namespace Test
                    {
                        public class MixinWithPublicMethod
                        {
                            public string PublicMethod()
                            {
                                return ""Public Method"";
                            }             
                        }

                        [CopaceticSoftware.pMixins.Attributes.pMixin(
                            Mixin = typeof (Test.MixinWithPublicMethod))]
                        public partial class Target1{}       

                        [CopaceticSoftware.pMixins.Attributes.pMixin(
                            Mixin = typeof (Test.MixinWithPublicMethod))]
                        public partial class Target2{}                       
                    }
                    ";
            }
        }

        [Test]
        public void CanCallPublicMethodOnBothTargets()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target1",
                    "PublicMethod")
                .ShouldEqual("Public Method");

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target2",
                    "PublicMethod")
                .ShouldEqual("Public Method");
        }
    }

}
