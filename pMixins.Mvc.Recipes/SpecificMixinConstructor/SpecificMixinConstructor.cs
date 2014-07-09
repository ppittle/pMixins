//----------------------------------------------------------------------- 
// <copyright file="SpecificMixinConstructor.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 9, 2014 10:52:31 PM</date> 
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

using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Infrastructure;

namespace pMixins.Mvc.Recipes.SpecificMixinConstructor
{
    public class Mixin
    {
        public Mixin(string constructorName = "Default")
        {
            ConstructorUsed = constructorName;
        }

        public string ConstructorUsed { get; private set; }
    }

    [pMixin(Mixin = typeof(Mixin), ExplicitlyInitializeMixin = true)]
    public partial class SpecificMixinConstructor
    {
        Mixin IMixinConstructorRequirement<Mixin>.InitializeMixin()
        {
            return new Mixin("Custom");
        }
    }
}
