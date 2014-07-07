//----------------------------------------------------------------------- 
// <copyright file="VirtualMethodOverride.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 7, 2014 6:47:28 PM</date> 
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

namespace pMixins.Mvc.Recipes.VirtualMethodOverride
{
    public class VirtualMemberMixin
    {
        public virtual string GetName()
        {
            return "Mixin";
        }
    }

    [pMixin(Mixin = typeof(VirtualMemberMixin))]
    public partial class VirtualMethodOverride
    {
        public VirtualMethodOverride()
        {
            this.___mixins.pMixins_Mvc_Recipes_VirtualMethodOverride_VirtualMemberMixin.GetNameFunc =
                () => "Target";
        }
    }

    public class Child : VirtualMethodOverride
    {
        public override string GetName()
        {
            return "Child";
        }
    }
}
