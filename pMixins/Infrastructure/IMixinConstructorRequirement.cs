//----------------------------------------------------------------------- 
// <copyright file="IMixinConstructorRequirement.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 4:10:26 PM</date> 
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
    /// Added by the pMixin Code Generator if a class is
    /// decorated with a <see cref="pMixinAttribute"/> where
    /// <see cref="pMixinAttribute.ExplicitlyInitializeMixin"/> is <c>True</c>.
    /// </summary>
    /// <remarks>
    /// It is generally recommended that this interface be implemented explicitly 
    /// as consumers of your class don't need it and shouldn't call it.
    /// </remarks>
    /// <typeparam name="TMixin">
    /// The type of the Mixin.  This corresponds to 
    /// <see cref="pMixinAttribute.Mixin"/>
    /// </typeparam>
    public interface IMixinConstructorRequirement<out TMixin>
    {
        /// <summary>
        /// Returns a new instance of <typeparamref name="TMixin"/>
        /// </summary>
        TMixin InitializeMixin();
    }
}
