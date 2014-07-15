//----------------------------------------------------------------------- 
// <copyright file="MixinDependencyIsInheritedByTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 15, 2014 10:55:24 AM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.MixinDependency
{
    public class MixinDependencyIsInheritedByTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                    using CopaceticSoftware.pMixins.Attributes;
                    using CopaceticSoftware.pMixins.Infrastructure;

                    namespace Test{

                        public class Dependency
                        {
                            public int GetNumber(){ return 0;}
                        }

                        public class Mixin : IMixinDependency<Dependency>
                        {
                            public Dependency Dependency { get; set; }

                            //This can be implemented explicitly
                            void IMixinDependency<Dependency>.OnDependencySet() { }

                            public int MixinMethod()
                            {
                                return 42 + Dependency.GetNumber();
                            }
                        }

                        [pMixin(Mixin = typeof(Mixin))]
                        public partial class Target : Dependency {}
                    }";
            }
        }

        [Test]
        public void CanExecuteMixinMethod()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            int getNumber = target.MixinMethod();

            getNumber.ShouldEqual(42);
        }
    }
}
