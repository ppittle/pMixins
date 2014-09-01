//----------------------------------------------------------------------- 
// <copyright file="InActionCarousel.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, September 1, 2014 5:51:54 PM</date> 
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
using pMixins.Mvc.Recipes.CastingAndConversionOperators;

namespace pMixins.Mvc.Recipes.InActionCarousel
{
    public interface ICanSwim { void Swim(); }
    public interface ICanFly  { void Fly();  }
    public interface ICanRun  { void Run();  }

    public class Swimmer : ICanSwim
    {
        public void Swim(){}
    }

    public class Flyer : ICanFly
    {
        public void Fly(){}
    }

    public class Runner : ICanRun
    {
        public void Run() { }
    }

    [pMixin(Mixin = typeof(Swimmer))]
    public partial class Penguin { }

    [pMixin(Mixin = typeof(Swimmer))]
    [pMixin(Mixin = typeof(Flyer))]
    public partial class FlyingFish { }

    [pMixin(Mixin = typeof(Flyer))]
    [pMixin(Mixin = typeof(Runner))]
    public partial class Pegasus { }
}
