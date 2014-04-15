//----------------------------------------------------------------------- 
// <copyright file="MixinConstDataMembersAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinConstDataMembersAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithConstDataMembers
                            {
                                public const string DataMember = ""const"";                                
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.MixinWithConstDataMembers))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanGetDataMemberValue()
        {
            CompilerResults
                .ExecutePropertyGet<string>(
                    "Test.Target",
                    "DataMember")
                .ShouldEqual("const");
        }
        
        [Test]
        public void CanNotSetDataMemberValue()
        {
            var targetInstance =
                CompilerResults.TryLoadCompiledType("Test.Target");

            Assert.True(null != targetInstance,
                "Could not load Target instance");

            var dataMemberProperty =
                targetInstance.GetType().GetProperty("DataMember");

            Assert.True(null != dataMemberProperty,
                "Could not load Data Member Property");

            Assert.True(false == dataMemberProperty.CanWrite,
                "Data Member property Can Write - but should not have a setter");
            
        }
    }
}
