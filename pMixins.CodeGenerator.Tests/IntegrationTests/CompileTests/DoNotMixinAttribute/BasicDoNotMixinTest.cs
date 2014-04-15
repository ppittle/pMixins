//----------------------------------------------------------------------- 
// <copyright file="BasicDoNotMixinTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, March 29, 2014 3:46:18 PM</date> 
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

using CopaceticSoftware.pMixin.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixin.CodeGenerator.Tests.IntegrationTests.CompileTests.DoNotMixinAttribute
{
    [TestFixture]
    public class BasicDoNotMixinTest : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        using CopaceticSoftware.pMixin.Attributes;

                        namespace Test{

                        public class Mixin
                        {                           
                            public int MixinMethod(){return 10;}

                            [DoNotMixin]
                            public int DoNotMixinMethod(){return 42;}
                        }

                        [pMixin(Mixin = typeof(Mixin))]                        
                        public partial class Target
                        {
                            //copy signature of do not mixin to make it easier to test.
                            public int DoNotMixinMethod(){return 700;}
                        }
                    }";
            }
        }

        [Test]
        public void CanCallMixinMethod()
        {
            CompilerResults
               .ExecuteMethod<int>(
                   "Test.Target",
                   "MixinMethod")
               .ShouldEqual(10); 
        }

        [Test]
        public void DoNotMixinMethodIsNotMixedIn()
        {
            CompilerResults
               .ExecuteMethod<int>(
                   "Test.Target",
                   "DoNotMixinMethod")
               .ShouldEqual(700); 
        }
    }
}
