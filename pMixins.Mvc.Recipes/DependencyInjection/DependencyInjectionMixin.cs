//----------------------------------------------------------------------- 
// <copyright file="DependencyInjectionMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 4, 2014 5:28:59 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Infrastructure;
using NUnit.Framework;

namespace pMixins.Mvc.Recipes.DependencyInjection
{
    public class DependencyInjectionMixin
    {
        public DependencyInjectionMixin(string activator)
        {
            Activator = activator;
        }

        public string Activator { get; private set; }
    }

    public class DependencyInjector : IMixinActivator
    {
        public T CreateInstance<T>(params object[] constructorArgs)
        {
            if (typeof (DependencyInjectionMixin).IsAssignableFrom(typeof (T)))
            {
                constructorArgs = constructorArgs.Union(new object[] {"DI"}).ToArray();
            }

            return new DefaultMixinActivator().CreateInstance<T>(constructorArgs);
        }
    }


    [pMixin(Mixin = typeof(DependencyInjectionMixin))]
    public partial class DependencyInjectionMixinExample
    {
    }

    [TestFixture]
    public class DependencyInjectionMixinExampleTest
    {
        [TestFixtureSetUp]
        public void MainSetup()
        {
            MixinActivatorFactory.SetCurrentActivator(new DependencyInjector());
        }

        [TestFixtureTearDown]
        protected void Cleanup()
        {
            MixinActivatorFactory.SetCurrentActivator(new DefaultMixinActivator());
        }

        [Test]
        public void CanCallActivator()
        {
            Assert.AreEqual(
                "DI",
                new DependencyInjectionMixinExample().Activator);
        }
    }
}
