//----------------------------------------------------------------------- 
// <copyright file="BasicConceptSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 22, 2013 10:04:30 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.StaticMixinInstance
{
    /// <summary>
    /// Also needs to return instructions on how to access 
    /// the mixin variable (ie __Mixin.ExampleMixin vs __mixins.ExampleMixin)
    /// </summary>
    public interface IMixinInstanceManagentStrategy
    {
        string GenerateCodeToInitializeMixin(string instanceName, Type mixinType);
    }

    public class DefaultMixinInstanceManagementStrategy : IMixinInstanceManagentStrategy
    {
        public string GenerateCodeToInitializeMixin(string instanceName, Type mixinType)
        {
            return
                string.Format(
                    @"public readonly Lazy<{0}> {1} = 
                    new Lazy<{0}>(
                        () => new DefaultMixinActivator().CreateInstance<{0}>());",
                    mixinType.FullName,
                    instanceName);
        }
    }

    public class StaticMixinInstanceManagementStrategy : IMixinInstanceManagentStrategy
    {
        public string GenerateCodeToInitializeMixin(string instanceName, Type mixinType)
        {
            return
                string.Format(
                    @"public static readonly Lazy<{0}> {1} = 
                    new Lazy<{0}>(
                        () => new DefaultMixinActivator().CreateInstance<{0}>());",
                    mixinType.FullName,
                    instanceName);
        }
    }

    public class ExtendedMixinAttribute : BasicMixinAttribute
    {
        public ExtendedMixinAttribute()
        {
            InstanceManagementStrategy = typeof (DefaultMixinInstanceManagementStrategy);
        }

        public Type InstanceManagementStrategy { get; set; }
    }

    public class ExampleMixin
    {
        public int SomeNumber { get; set; }
    }

    [ExtendedMixin(Target = typeof(ExampleMixin),
        InstanceManagementStrategy = typeof(StaticMixinInstanceManagementStrategy))]
    public partial class StaticMixinInstanceSpec { }

/*/////////////////////////////////////////
    /// Generated Code
    /////////////////////////////////////////*/

    public partial class StaticMixinInstanceSpec
    {
        private class __Mixins
        {
            public static readonly Lazy<ExampleMixin> _ExampleMixin = new Lazy<ExampleMixin>(
                () => new DefaultMixinActivator().CreateInstance<ExampleMixin>());
        }

        public int SomeNumber 
        { 
            get
            {
                return __Mixins._ExampleMixin.Value.SomeNumber;
            } 
            set { __Mixins._ExampleMixin.Value.SomeNumber = value; } 
        }

        public static implicit operator ExampleMixin(StaticMixinInstanceSpec spec)
        {
            return __Mixins._ExampleMixin.Value;
        }
    }
}