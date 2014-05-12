//----------------------------------------------------------------------- 
// <copyright file="MixinBaseClassMembersAreInjectedIntoTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 4:29:55 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests
{
    public class MixinBaseClassMembersAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class BaseClass
                            {
                                public string PublicMethod(int i)
                                {
                                    return i.ToString();
                                } 
                            }

                            public class MixinChild : BaseClass
                            {                                                           
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinChild))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanCallBaseClassMethod()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "PublicMethod",
                    ReflectionHelper.DefaultBindingFlags,
                    42)
                .ShouldEqual("42");
        }
    }
}
