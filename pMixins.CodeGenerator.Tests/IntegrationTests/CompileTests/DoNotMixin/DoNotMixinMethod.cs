//----------------------------------------------------------------------- 
// <copyright file="DoNotMixinMethod.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, April 21, 2014 6:26:54 PM</date> 
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

using System.Reflection;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.DoNotMixin
{
    [TestFixture]
    public class DoNotMixinMethod : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                @"
                    using CopaceticSoftware.pMixins.Attributes;

                    namespace Test
                    {
                        public class Mixin
                        {
                            public string MixinMethod()
                            {
                                return ""MixinMethod"";
                            }

                            [DoNotMixin]
                            public string DoNotMixinMethod()
                            {
                                return ""DoNotMixinMethod"";
                            }
                        }
                        
                        [pMixin(Mixin = typeof(Mixin))]
                        public partial class Target{}
                    }
                ";
            }
        }

        [Test]
        public void CanCallMixinMethod()
        {
            CompilerResults
                 .ExecuteMethod<string>(
                     "Test.Target",
                     "MixinMethod")
                 .ShouldEqual("MixinMethod"); 
        }

        [Test]
        public void DoNotMixinMethodShouldNotBePresent()
        {
            CompilerResults
                .TryLoadCompiledType("Test.Target")
                .GetType()
                .GetMethod("DoNotMixinMethod", BindingFlags.Instance | BindingFlags.Public)
                .ShouldBeNull();
                
        }
    }
}
