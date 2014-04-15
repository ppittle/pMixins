//----------------------------------------------------------------------- 
// <copyright file="AsIsHelperTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 23, 2014 1:33:10 PM</date> 
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
using CopaceticSoftware.pMixins.ConversionOperators;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests
{
    [TestFixture]
    public class AsIsHelperTest : GenerateCodeAndCompileTestBase
    {
        public class AsIsMixin{}

        protected override string SourceCode
        {
            get
            {
                return
                    string.Format(
                    @"
                namespace Test{{
                    
                    public abstract class Mixin{{}}

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof ({0}))]
                    public partial class Target
                    {{                        
                    }}
                }}",
                typeof(AsIsMixin).FullName.Replace("+", "."));
            }
        }
        
        [Test]
        public void CanUseAsOperator()
        {
            CompilerResults.TryLoadCompiledType("Test.Target")
                           .As<AsIsMixin>().ShouldNotBeNull();
        }

        [Test]
        public void CanUseIsOperator()
        {
            CompilerResults.TryLoadCompiledType("Test.Target")
                           .Is<AsIsMixin>().ShouldBeTrue();
        }
    }
}
