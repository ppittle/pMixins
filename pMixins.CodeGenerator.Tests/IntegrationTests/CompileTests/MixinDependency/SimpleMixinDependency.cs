//----------------------------------------------------------------------- 
// <copyright file="SimpleMixinDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 3:49:01 PM</date> 
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
    public class SimpleMixinDependency : GenerateCodeAndCompileTestBase
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

                        public interface IDependency
                        {
                            int GetNumber();
                        }

                        public class Mixin : IMixinDependency<IDependency>
                        {
                            public IDependency Dependency { get; set; }

                            //This can be implemented explicitly
                            void IMixinDependency<IDependency>.OnDependencySet() { }

                            public int MixinMethod()
                            {
                                return 42 + Dependency.GetNumber();
                            }
                        }

                        [pMixin(Mixin = typeof(Mixin))]
                        public partial class Target
                        {
                            int IDependency.GetNumber()
                            {
                                return 0;
                            }
                        }

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
