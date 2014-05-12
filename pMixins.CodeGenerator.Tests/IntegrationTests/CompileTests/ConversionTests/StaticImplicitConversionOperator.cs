//----------------------------------------------------------------------- 
// <copyright file="StaticImplicitConversionOperator.cs" company="Copacetic Software"> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests
{
    public class ExampleMixin
    {}

    [TestFixture]
    public class StaticImplicitConversionOperator : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return string.Format(
                    @"
                    namespace Test{{
                        public class TestShim{{
                            public bool DoConversionTest()
                            {{
                                //Explicit cast
                                var explicitCast = ({0})new Target();
                                if (null == explicitCast)
                                    throw new System.Exception(""Explicit Cast Failed"");
    
                                //Implicit cast
                                {0} implicitCast = new Target();          
                                if (null == implicitCast)
                                    throw new System.Exception(""Implicit Cast Failed"");
                                
                                return true;
                               
                            }}
                        }}

                        [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof ({0}))]
                        public partial class Target{{}}
                    }}",
                    typeof (ExampleMixin).FullName);
            }
        }

        [Test]
        public void CanConvertTargetToMixin()
        {
            CompilerResults.ExecuteMethod<bool>(
                "Test.TestShim",
                "DoConversionTest")
                .ShouldBeTrue();
        }
    }
}
