//----------------------------------------------------------------------- 
// <copyright file="HostCanOverrideAndExposeVirtualMixinMembersSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 6:45:04 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostCanOverrideAndExposeVirtualMixinMembers
{
    /// <summary>
    /// Covered in:
    ///    <see cref="MixinVirtualMembersAreInjectedAsVirtual"/>
    /// </summary>
    public class MixinWithVirtualMember
    {
        public virtual string PrettyPrint(string name)
        {
            return "MixinWithVirtualMember_" + name;
        }
    }

    [BasicMixin(Target = typeof(MixinWithVirtualMember))]
    public partial class HostCanOverrideAndExposeVirtualMixinMembersSpec
    {
        public HostCanOverrideAndExposeVirtualMixinMembersSpec()
        {
            // Need a better way to do the overload
            __mixins._ExampleMixin.Value.PrettyPrint1Func = s =>
                "HostCanOverrideAndExposeVirtualMixinMembersSpec_" + s;
        }   
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public interface IVirtualMembersShim
    {
        string PrettyPrint(string name);
    }

    public class MixinWithVirtualMemberWrapper : MixinWithVirtualMember
    {
        private IVirtualMembersShim _foo;

        public MixinWithVirtualMemberWrapper(IVirtualMembersShim foo)
        {
            _foo = foo;
            PrettyPrint1Func = s => base.PrettyPrint(s);
        }

        /// <summary>
        /// Call to base is stored in Func
        /// </summary>
        public Func<string,string> PrettyPrint1Func { get; set; }

        public override string PrettyPrint(string name)
        {
            return _foo.PrettyPrint(name);
        }
    }

    public partial class HostCanOverrideAndExposeVirtualMixinMembersSpec : IVirtualMembersShim
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(HostCanOverrideAndExposeVirtualMixinMembersSpec host)
            {
                _ExampleMixin = new Lazy<MixinWithVirtualMemberWrapper>(
                    () => new DefaultMixinActivator().CreateInstance<MixinWithVirtualMemberWrapper>(host));
                        
            }

            public readonly Lazy<MixinWithVirtualMemberWrapper> _ExampleMixin;
        }

        private HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins ___mixins;

        private HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins __mixins
        {
            get
            {
                if (null == ___mixins)
                    ___mixins = new HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins(this);

                return ___mixins;
            }
        }

        //expose GetString as virtual so it can be overriden by a child class
        public virtual string PrettyPrint(string name)
        {
            return __mixins._ExampleMixin.Value.PrettyPrint1Func(name);
        }

        public static implicit operator MixinWithVirtualMemberWrapper(HostCanOverrideAndExposeVirtualMixinMembersSpec host)
        {
            return host.__mixins._ExampleMixin.Value;
        }
    }
}