//----------------------------------------------------------------------- 
// <copyright file="HostInheritsMixiInterfacesSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, September 3, 2013 5:02:54 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsMixinInterfaces
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinInterfacesAreInjectedIntoTarget"/>
    /// </summary>
    public interface IMixinParent
    {
        void ParentMethod();
    }

    public interface IMixinChild : IMixinParent
    {
        string ChildMethod(DateTime dt);
    }

    public interface IMixinOther
    {
        void OtherMethod();
    }

    public class ExampleMixin : IMixinChild, IMixinOther
    {
        public void ParentMethod(){}
        public string ChildMethod(DateTime dt) { return "Hello World"; }
        public void OtherMethod(){}

        public void ClassSpecificMethod(){}
    }

    [BasicMixin(Target = typeof(ExampleMixin))]
    public partial class HostInheritsMixinInterfacesSpec
    {
    }

/*/////////////////////////////////////////
    /// Generated Code
    /////////////////////////////////////////*/

    public partial class HostInheritsMixinInterfacesSpec : IMixinChild, IMixinOther
    {
        private class __Mixins
        {
            public readonly Lazy<ExampleMixin> _ExampleMixin = new Lazy<ExampleMixin>(
                () => new DefaultMixinActivator().CreateInstance<ExampleMixin>());
        }

        private readonly __Mixins __mixins = new __Mixins();

        public void ParentMethod() { __mixins._ExampleMixin.Value.ParentMethod(); }
        public string ChildMethod(DateTime dt) { return __mixins._ExampleMixin.Value.ChildMethod(dt); }
        public void OtherMethod() { __mixins._ExampleMixin.Value.OtherMethod(); }

        public void ClassSpecificMethod() { __mixins._ExampleMixin.Value.ClassSpecificMethod(); }

        public static implicit operator ExampleMixin(HostInheritsMixinInterfacesSpec spec)
        {
            return spec.__mixins._ExampleMixin.Value;
        }
    }
}