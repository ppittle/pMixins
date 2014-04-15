//----------------------------------------------------------------------- 
// <copyright file="MixinWithProtectedConstructorAndParametersSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, October 11, 2013 4:20:03 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinWithProtectedConstructor
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinDoesNotHaveParameterlessConstructor"/>
    /// </summary>
    public class MixinWithProtectedConstructorWithParametersMixin
    {
        private readonly string _name;
        
        protected MixinWithProtectedConstructorWithParametersMixin(string name)
        {
            _name= name;
        }

        public string PrettyPrintName()
        {
            return "Pretty " + _name;
        }
    }

    [BasicMixin(Target = typeof(MixinWithProtectedConstructorWithParametersMixin), 
        RequiresInitialization = true)]
    public partial class MixinWithProtectedConstructorAndParametersSpec
    {
        private string _name;
        public MixinWithProtectedConstructorAndParametersSpec(string name)
        {
            _name = name;
        }

        public HostCanAccessProtectedMembersInMixinMixinWrapper InitializeMixinWithProtectedConstructorWithParametersMixin()
        {
            return new HostCanAccessProtectedMembersInMixinMixinWrapper(_name);
        }
    }


/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public interface IMixinWithProtectedConstructorWithParametersMixinRequirements
    {
        HostCanAccessProtectedMembersInMixinMixinWrapper InitializeMixinWithProtectedConstructorWithParametersMixin();
    }

    public class HostCanAccessProtectedMembersInMixinMixinWrapper : MixinWithProtectedConstructorWithParametersMixin
    {
        public HostCanAccessProtectedMembersInMixinMixinWrapper(string name) : base(name)
        {
        }
    }

    public partial class MixinWithProtectedConstructorAndParametersSpec :
        IMixinWithProtectedConstructorWithParametersMixinRequirements
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(MixinWithProtectedConstructorAndParametersSpec host)
            {
                _ExampleMixin = new Lazy<MixinWithProtectedConstructorWithParametersMixin>(
                    () => host.InitializeMixinWithProtectedConstructorWithParametersMixin());
            }

            public readonly Lazy<MixinWithProtectedConstructorWithParametersMixin> _ExampleMixin;
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

        public string PrettyPrintName()
        {
            return __mixins._ExampleMixin.Value.PrettyPrintName();
        }
    }
}