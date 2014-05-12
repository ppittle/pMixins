//----------------------------------------------------------------------- 
// <copyright file="MixinIsCreatedWithDependencyInjection.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, February 26, 2014 6:00:29 PM</date> 
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

using System.Linq;
using CopaceticSoftware.pMixins.Infrastructure;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    public class MixinIsCreatedWithDependencyInjection : GenerateCodeAndCompileTestBase
    {
        public class DependencyInjectorMixinActivator : IMixinActivator
        {
            public T CreateInstance<T>(params object[] constructorArgs)
            {
                if (typeof (T).FullName.Contains("DI") &&
                    !typeof(T).FullName.Contains("Master"))
                {
                    var newArgs =
                        constructorArgs.ToList();

                    newArgs.Add("DI!");

                    constructorArgs = newArgs.ToArray();
                }

                return new DefaultMixinActivator().CreateInstance<T>(constructorArgs);
            }
        }

        protected override string SourceCode
        {
            get
            {
                return
                    @"
                using System.Linq;

                namespace Test{                   

                    public class DIMixin
                    {
                        private string _s;

                        public DIMixin(string s)
                        {
                            _s = s;
                        }

                        public string Method()
                        {
                            return _s;
                        }            
                    }

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.DIMixin))]
                    public partial class Target
                    {}
                }";
            }
        }

        public override void MainSetup()
        {
            MixinActivatorFactory.SetCurrentActivator(new DependencyInjectorMixinActivator());

            base.MainSetup();
        }

        protected override void Cleanup()
        {
            MixinActivatorFactory.SetCurrentActivator(new DefaultMixinActivator());
        }

        [Test]
        public void CanCallMixedInMethod()
        {
            CompilerResults.ExecuteMethod<string>(
                "Test.Target",
                "Method")
                .ShouldEqual("DI!");
        }
    }
}
