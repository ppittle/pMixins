//----------------------------------------------------------------------- 
// <copyright file="HostInheritsAttributesSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, November 4, 2013 9:43:59 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsAttributes
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinAttributesAreInjectedIntoTarget"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class InheritedAttribute : Attribute{}

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class NonInheritedAttribute : Attribute{}

    [Inherited]
    [NonInherited]
    public class HostInheritsAttributesMixin
    {
        [Inherited]
        [NonInherited]
        public void Foo(){}
    }

    [BasicMixin(Target = typeof(HostInheritsAttributesMixin))]
    public partial class HostInheritsAttributesSpec
    {
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    [Inherited]
    public partial class HostInheritsAttributesSpec
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(HostInheritsAttributesSpec host)
            {
                _ExampleMixin = new Lazy<HostInheritsAttributesSpec>(
                    () => new DefaultMixinActivator().CreateInstance<HostInheritsAttributesSpec>());
            }

            public readonly Lazy<HostInheritsAttributesSpec> _ExampleMixin;
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

        [Inherited]
        public void Foo()
        {
            __mixins._ExampleMixin.Value.Foo();
        }
    }
}