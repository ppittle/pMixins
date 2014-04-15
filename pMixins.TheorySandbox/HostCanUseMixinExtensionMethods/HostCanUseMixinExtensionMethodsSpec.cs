//----------------------------------------------------------------------- 
// <copyright file="HostCanUseMixinExtensionMethodsSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, October 14, 2013 12:41:21 AM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.HostCanUseMixinExtensionMethods
{
    public class HostCanUseMixinExtensionMethodsMixin
    {
    }

    public static class HostCanUseMixinExtensionMethodsMixinExtensionMethods
    {
        public static string PrettyPrintNameExtension(this HostCanUseMixinExtensionMethodsMixin mixin, string name)
        {
            return "Extension_" + name;
        }
    }


    [BasicMixin(Target = typeof(HostCanUseMixinExtensionMethodsMixin),
        GenerateExtensionMethodWrappers = true)]
    public partial class HostCanUseMixinExtensionMethodsSpec
    {
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public partial class HostCanUseMixinExtensionMethodsSpec
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(HostCanUseMixinExtensionMethodsSpec host)
            {
                _ExampleMixin = new Lazy<HostCanUseMixinExtensionMethodsMixin>(
                    () => new DefaultMixinActivator().CreateInstance<HostCanUseMixinExtensionMethodsMixin>());
            }

            public readonly Lazy<HostCanUseMixinExtensionMethodsMixin> _ExampleMixin;
        }

        private __Mixins ___mixins;

        private __Mixins __mixins
        {
            get
            {
                if (null == ___mixins)
                    ___mixins = new __Mixins(this);

                return ___mixins;
            }
        }

        public static implicit operator HostCanUseMixinExtensionMethodsMixin(HostCanUseMixinExtensionMethodsSpec host)
        {
            return host.__mixins._ExampleMixin.Value;
        }
    }

    /// <summary>
    /// - Use the same namespace as the original extension class (<see cref="HostCanUseMixinExtensionMethodsMixinExtensionMethods"/>)
    /// - Mark as partial incase there are naming collisions 
    /// - What should class name be called?  Will never be seen in code, but will show up in intellisense
    /// </summary>
    public static partial class __HostCanUseMixinExtensionMethodsSpecExtensionMethods
    {
        public static string PrettyPrintNameExtension(this HostCanUseMixinExtensionMethodsSpec mixin, string name)
        {
            // Use default calling convention, don't use extension.  Might be problems w/ nulls and/or casting
            return HostCanUseMixinExtensionMethodsMixinExtensionMethods.PrettyPrintNameExtension(mixin, name);
        }
    }
}