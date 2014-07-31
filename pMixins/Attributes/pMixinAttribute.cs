//----------------------------------------------------------------------- 
// <copyright file="pMixinAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, November 9, 2013 6:07:43 PM</date> 
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
using CopaceticSoftware.pMixins.Infrastructure;
using CopaceticSoftware.pMixins.Interceptors;

namespace CopaceticSoftware.pMixins.Attributes
{
    //TODO: Method collision resolution strategy?
    /// <summary>
    /// The primary pMixin attribute, used to inject the members of 
    /// <see cref="Mixin"/> into the Target.
    /// </summary>
    /// <example>
    /// In this scenario the members of Foo are mixed
    /// into Target:
    /// <code>
    /// <![CDATA[
    /// public class Foo{
    ///    public void Method1(){}
    ///    public void Method2(){}
    /// }
    ///
    /// [pMixin(Mixin = typeof(Foo))]
    /// public partial class Target{}
    /// ]]></code>
    /// Generated file:
    /// <code>
    /// <![CDATA[
    /// public partial class Target{
    ///    public void Method1()
    ///    {
    ///       //Implementation omitted from example
    ///    }
    /// 
    ///    public void Method2()
    ///    {
    ///       //Implementation omitted from example
    ///    }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class pMixinAttribute : Attribute, IPMixinAttribute
    {
        public pMixinAttribute()
        {
            Masks = new Type[0];
            Interceptors = new Type[0];
            EnableSharedRequirementsInterface = true;
        }

        /// <summary>
        /// Sets the type to be Mixed in
        /// </summary>
        /// <example>
        /// In this scenario the members of Foo are mixed
        /// into Target:
        /// <code>
        /// <![CDATA[
        /// public class Foo{
        ///    public void Method1(){}
        ///    public void Method2(){}
        /// }
        ///
        /// [pMixin(Mixin = typeof(Foo))]
        /// public partial class Target{}
        /// ]]></code>
        /// Generated file:
        /// <code>
        /// <![CDATA[
        /// public partial class Target{
        ///    public void Method1()
        ///    {
        ///       //Implementation ommited from example
        ///    }
        /// 
        ///    public void Method2()
        ///    {
        ///       //Implementation ommited from example
        ///    }
        /// }
        /// ]]></code>
        /// </example>
        public Type Mixin { get; set; }

        /// <summary>
        /// Optional array of <see cref="Type"/>s 
        /// used to limit which members from <see cref="Mixin"/>
        /// are mixed in. 
        /// </summary>
        /// <example>
        /// In this scenario Foo has 2 methods, but IBar only has 1.
        /// Therefore only 1 method is mixed into Target:
        /// <code>
        /// <![CDATA[
        /// public interface IBar{
        ///     void Method1();
        /// }
        /// 
        /// public class Foo : IBar{
        ///    public void Method1(){}
        ///    public void Method2(){}
        /// }
        /// 
        /// [pMixin(Mixin = typeof(Foo), Masks = new Type[]{ typeof(IBar) })]
        /// public partial class Target{}
        /// ]]></code>
        /// Generated file:
        /// <code>
        /// <![CDATA[
        /// public partial class Target : IBar{
        ///    public void Method1()
        ///    {
        ///       //Implementation ommited from example
        ///    }
        /// }
        /// ]]></code>
        /// </example>
        public Type[] Masks { get; set; }

        /// <summary>
        /// Collection of Interceptor types.  Interceptors can register
        /// for Join Point Events on the <see cref="Mixin"/>'s members.
        /// Interceptors must inherit from
        /// <see cref="IMixinInterceptor"/> and be activatable by the 
        /// <see cref="MixinActivatorFactory"/>.
        /// </summary>
        public Type[] Interceptors { get; set; }

        /// <summary>
        /// If <c>true</c>, mixed in members with the same signature should
        /// be combined into a single interface (ISharedRequirements)
        /// so that the Target only need to implement them once.
        /// If <c>false</c> all members are added individually to the Mixin
        /// specific requirements interface and must be separately implemented
        /// by the Target.
        /// 
        /// Default is <c>true</c>.
        /// </summary>
        public bool EnableSharedRequirementsInterface { get; set; }

        /// <summary>
        /// When set to <c>True</c> a method is added to the Target class
        /// that allows the consumer to explicitly initialize the Mixin.
        /// This can be useful if the Mixin has multiple constructors and 
        /// using a particular constructor is desired.  
        ///  Default is <c>False</c>.
        /// </summary>
        public bool ExplicitlyInitializeMixin { get; set; }

        #region Implement in later version
        /*
        /// <summary>
        /// An optional <see cref="Type"/> that can initialize the <see cref="Mixin"/>.
        /// 
        /// The <see cref="Type"/> must meet the following requirements:
        ///    - public parameterless constructor
        ///    - must implement <see cref="IMixinActivator"/>
        /// </summary>
        public Type ExplicitInitializer { get; set; }
         
        /// <summary>
        /// Sets how much logging is done in the generated file.
        /// Default is 
        /// <see cref="Attributes.LoggingVerbosity.Error"/>
        /// </summary>
        public LoggingVerbosity LoggingVerbosity { get; set; }

        /// <summary>
        /// Indicates if all loaded assemblies should be scanned for extension
        /// methods.  Extension methods will then be copied to work with the 
        /// host type.  Default is <c>False</c>.
        /// </summary>
        public bool GenerateExtensionMethodWrappers { get; set; } 
         */
        #endregion
    }
}
