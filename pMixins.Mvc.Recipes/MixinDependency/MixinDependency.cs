//----------------------------------------------------------------------- 
// <copyright file="MixinDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 17, 2014 2:49:21 PM</date> 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Infrastructure;
using NUnit.Framework;

namespace pMixins.Mvc.Recipes.MixinDependency
{
    public class Decorator<T> : IMixinDependency<T>
        where T : class
    {
        public T Dependency { get; set; }

        public int GetNumber()
        {
            return 42;
        }

        void IMixinDependency<T>.OnDependencySet(){}
    }

    public class OtherDecorator<T> : IMixinDependency<T>
        where T : class
    {
        public int GetOtherNumber()
        {
            return 24;
        }

        public T Dependency { get; set; }

        void IMixinDependency<T>.OnDependencySet() { }
    } 

    public abstract class Repository
    {
        public abstract string Name { get; set; }
    }

    [pMixin(Mixin = typeof(Repository))]
    [pMixin(Mixin = typeof(Decorator<Repository>))]
    [pMixin(Mixin = typeof(OtherDecorator<Decorator<Repository>>))]
    public partial class MixinDependency
    {
        public string NameImplementation { get; set; }
    }

    [TestFixture]
    public class MixinDependencyTest
    {
        [Test]
        public void CallMethods()
        {
            
        }
    }
}
