//----------------------------------------------------------------------- 
// <copyright file="ConversionOperators.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 16, 2014 2:04:27 PM</date> 
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

namespace pMixins.Mvc.Recipes.CastingAndConversionOperators
{
    public interface ISomeInterface
    {
        string InterfaceMethod();
    }

    public abstract class SomeBaseClass
    {
        public abstract string BaseClassMethod();
    }

    public class Mixin : SomeBaseClass, ISomeInterface
    {
        public override string BaseClassMethod()
        {
            return "Base Class";
        }

        public string InterfaceMethod()
        {
            return "Interface";
        }
    }
    
    [pMixin(Mixin = typeof(Mixin))]
    public partial class CastingAndConversionOperators
    {
    }
}
