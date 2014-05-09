//----------------------------------------------------------------------- 
// <copyright file="InterceptorMixinRequirementAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 3:47:45 PM</date> 
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
using CopaceticSoftware.pMixins.Interceptors;

namespace CopaceticSoftware.pMixins.Attributes
{
    /// <summary>
    /// Allows an <see cref="IMixinInterceptor"/> to add an 
    /// additional Mixin to the Target.
    /// </summary>
    /// <remarks>
    /// This is functionally equivalent to the Target adding
    /// an extra <see cref="pMixinAttribute"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
        AllowMultiple = true, Inherited = true)]
    public class InterceptorMixinRequirementAttribute : Attribute, IPMixinAttribute
    {
        /// <summary>
        /// Sets the type to be Mixed in
        /// </summary>
        /// <example>
        /// See  <see cref="pMixinAttribute.Mixin"/> for an example.
        /// </example>
        public Type Mixin { get; set; }
    }
}
