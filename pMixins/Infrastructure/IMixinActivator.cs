//----------------------------------------------------------------------- 
// <copyright file="IMixinActivator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, January 28, 2014 2:13:06 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.Infrastructure
{
    /// <summary>
    /// Describes a class that is able to create instances of 
    /// Types listed in <see cref="pMixinAttribute.Mixin"/>.
    /// </summary>
    /// <remarks>
    /// The pMixin Code Generator will use objects of this type in generated code.
    /// </remarks>
    public interface IMixinActivator
    {
        T CreateInstance<T>(params object[] constructorArgs);
    }

    /// <summary>
    /// Default <see cref="IMixinActivator"/> that able to create instances of 
    /// Types listed in <see cref="pMixinAttribute.Mixin"/>.
    /// </summary>
    public class DefaultMixinActivator : IMixinActivator
    {
        /// <summary>
        /// Creates a new instance of <typeparam name="T" /> 
        /// using <see cref="Activator"/>.
        /// </summary>
        /// <typeparam name="T">They <see cref="Type"/> to create. </typeparam> 
        /// <param name="constructorArgs">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. 
        /// If <paramref name="constructorArgs"/> is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked. </param> 
        /// <returns>An instance of <typeparamref name="T" /></returns>
        public T CreateInstance<T>(params object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(typeof(T), constructorArgs);
        }
    }
}
