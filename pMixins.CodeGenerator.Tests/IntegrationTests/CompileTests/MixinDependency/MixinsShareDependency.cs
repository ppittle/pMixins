//----------------------------------------------------------------------- 
// <copyright file="MixinsShareDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 3:57:16 PM</date> 
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
    public class MixinsShareDependency : GenerateCodeAndCompileTestBase
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

                        public class Mixin1 : IMixinDependency<Dependency>
                        {
                            public Dependency Dependency { get; set; }

                            //This can be implemented explicitly
                            void IMixinDependency<Dependency>.OnDependencySet() { }

                            public int MixinMethod1()
                            {
                                return 42 + Dependency.GetNumber();
                            }
                        }

                        public class Mixin2 : IMixinDependency<Dependency>
                        {
                            public Dependency Dependency { get; set; }

                            //This can be implemented explicitly
                            void IMixinDependency<Dependency>.OnDependencySet() { }

                            public int MixinMethod2()
                            {
                                return 24 + Dependency.GetNumber();
                            }
                        }

                        [pMixin(Mixin = typeof(Mixin1))]
                        [pMixin(Mixin = typeof(Dependency))]
                        [pMixin(Mixin = typeof(Mixin2))]
                        public partial class Target {}
                    }";
            }
        }

        [Test]
        public void CanExecuteMixin1Method()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            int getNumber = target.MixinMethod1();

            getNumber.ShouldEqual(42);
        }

        [Test]
        public void CanExecuteMixin2Method()
        {
            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            int getNumber = target.MixinMethod2();

            getNumber.ShouldEqual(24);
        }
        
    }
}
