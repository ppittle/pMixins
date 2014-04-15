//----------------------------------------------------------------------- 
// <copyright file="HostInheritsMethodsMixinBaseClassMethodsSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 5:15:26 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsMethodsMixinBaseClassMethods
{
    /// <summary>
    /// Covered in
    ///     <see cref="MixinBaseClassMembersAreInjectedIntoTarget"/>
    /// </summary>
    public class ExampleBaseMixin
    {
        public string PrettyPrint(string name)
        {
            return "Base Pretty " + name;
        }
    }

    public class ExampleMixin : ExampleBaseMixin{}


    [BasicMixin(Target = typeof(ExampleMixin))]
    public partial class HostInheritsMethodsMixinBaseClassMethodsSpec
    {
    }


/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public partial class HostInheritsMethodsMixinBaseClassMethodsSpec
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public readonly Lazy<ExampleMixin> _ExampleMixin = new Lazy<ExampleMixin>(
                () => new DefaultMixinActivator().CreateInstance<ExampleMixin>());
        }

        private readonly __Mixins __mixins = new __Mixins();

        public string PrettyPrint(string name)
        {
            return __mixins._ExampleMixin.Value.PrettyPrint(name);
        }
    }
}