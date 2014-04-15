//----------------------------------------------------------------------- 
// <copyright file="IContainMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 23, 2014 1:33:07 PM</date> 
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

namespace CopaceticSoftware.pMixins.ConversionOperators
{
    /// <summary>
    /// Classes that are decorated with <see cref="pMixinAttribute"/> will
    /// automatically implement this interface by the pMixin Code Generator.  It
    /// is used by the <see cref="AsIsHelper"/> to provide equivalent functions for the
    /// <c>Is</c> and <c>As</c> operators.  See <see cref="AsIsHelper"/> for additional
    /// information.
    /// </summary>
    /// <remarks>
    /// This interface is intended for use by pMixin and should not be used in your code directly.
    /// </remarks>
    /// <typeparam name="TMixin">The type corresponding to <see cref="pMixinAttribute.Mixin"/>.
    /// </typeparam>
    public interface IContainMixin<out TMixin>
    {
        /// <summary>
        /// Returns the underlying instance of <see cref="pMixinAttribute.Mixin"/>
        /// </summary>
        TMixin MixinInstance { get; }
    }
}
