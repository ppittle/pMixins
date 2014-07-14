//----------------------------------------------------------------------- 
// <copyright file="IMixinDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 3:48:54 PM</date> 
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

namespace CopaceticSoftware.pMixins.Infrastructure
{
    /// <summary>
    /// Allows a Mixin to indicate that to be used, the Target
    /// must provide <typeparamref name="T"/>, either by directly implementing
    /// or extending <typeparamref name="T"/> or by mixing in another Mixin
    /// that is <typeparamref name="T"/> or provides <typeparamref name="T"/>
    /// </summary>
    /// <remarks>
    /// If the Target does not mixin <typeparamref name="T"/> and does not
    /// implement or extend <typeparamref name="T"/> and <typeparamref name="T"/>
    /// is an interface, <typeparamref name="T"/> will be automatically added to the 
    /// list of interfaces Target implements in the code-behind.  If <typeparamref name="T"/>
    /// is a class, pMixins will add an error to Visual Studio and terminate code generation.
    /// </remarks>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public interface IDependency
    /// {
    ///     int GetNumber();
    /// }
    /// 
    /// public class Mixin : IMixinDependency<IDependency>
    /// {
    ///     public IDependency Dependency { get; set; }
    /// 
    ///     //This can be implemented explicitly
    ///     void IMixinDependency<IDependency>.OnDependencySet() { }
    /// 
    ///     public int MixinMethod()
    ///     {
    ///         return 42 + Dependency.GetNumber();
    ///     } 
    /// }
    /// 
    /// [pMixin(Mixin = typeof(Mixin))]
    /// public partial class Target
    /// {
    ///     int IDependency.GetNumber()
    ///     {
    ///         return 0;
    ///     }
    /// }
    /// ]]>
    /// </code> 
    /// </example>
    /// <typeparam name="T">The Type the Mixin depends on.</typeparam>
    public interface IMixinDependency<T> where T : class
    {
        /// <summary>
        /// Property will be injected by the pMixin infrastructure.
        /// Do NOT use in Constructor, use the event <see cref="OnDependencySet"/>
        /// </summary>
        /// <remarks>
        /// This property is marked with <see cref="DoNotMixinAttribute"/> so it
        /// will not be added to a Target.
        /// </remarks>
        [DoNotMixin]
        T Dependency { get; set; }

        /// <summary>
        /// Called after the <see cref="Dependency"/>
        /// is set.
        /// </summary>
        /// <remarks>
        /// This property is marked with <see cref="DoNotMixinAttribute"/> so it
        /// will not be added to a Target.
        /// </remarks>
        [DoNotMixin]
        void OnDependencySet();
    }
}
