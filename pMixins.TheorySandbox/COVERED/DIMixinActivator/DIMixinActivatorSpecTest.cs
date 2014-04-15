//----------------------------------------------------------------------- 
// <copyright file="DIMixinActivatorSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, October 14, 2013 2:57:52 PM</date> 
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

using System;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;
using NBehave.Spec.NUnit;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.DIMixinActivator
{
    /// <summary>
    /// This is covered in :
    ///     <see cref="MixinIsCreatedWithDependencyInjection"/>, albeit
    ///     the implementation has evolved significantly.
    /// </summary>
    [TestFixture]
    public class DIMixinActivatorSpecTest : SpecTestBase
    {
        private class RandomDependency : IRandomDependency
        {
            public string PrettyPrintName(string name)
            {
                return "Random_" + name;
            }
        }

        private DIMixinActivatorSpec _spec;

        protected override void Establish_context()
        {
            var kernel = new StandardKernel(new NinjectSettings
            {
                InjectNonPublic = true
            });
            kernel.Bind<IRandomDependency>().To<RandomDependency>();

            DIPropertyInjectionActivator.kernel = kernel;

            _spec = new DIMixinActivatorSpec();
                //kernel.Get<DIMixinActivatorSpec>();
        }

        /// <summary>
        /// This is covered in :
        ///     <see cref="MixinIsCreatedWithDependencyInjection.CanCallMixedInMethod"/>, albeit
        ///     the implementation has evolved significantly.
        /// </summary>
        [Test]
        public void Can_Call_DI_Method()
        {
            _spec.PrettyPrintName("HelloWorld").ShouldNotBeEmpty();

            Console.WriteLine(_spec.PrettyPrintName("HelloWorld"));
        }


    }
}
