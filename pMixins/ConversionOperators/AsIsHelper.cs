//----------------------------------------------------------------------- 
// <copyright file="AsIsHelper.cs" company="Copacetic Software"> 
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

using System;

namespace CopaceticSoftware.pMixins.ConversionOperators
{
    /// <summary>
    /// Provides equivalent functions for the
    /// <c>Is</c> and <c>As</c> operators that support Mixed in types.
    /// </summary>
    /// <remarks>
    /// If necessary, this class can be extended with a custom 
    /// <see cref="IAsIsImplementation"/> by setting the 
    /// <see cref="AsIsImplementationFactory"/>
    /// </remarks>
    /// <remarks>
    /// Mixins will not natively support as/is operators 
    /// http://stackoverflow.com/questions/18390664/c-sharp-implicit-conversion-operator-and-is-as-operator,
    /// </remarks>
    public static class AsIsHelper
    {
        /// <summary>
        /// Provides the actual functionality for 
        /// performing <c>as</c> / <c>is</c> operations.
        /// </summary>
        public interface IAsIsImplementation
        {
            /// <summary>
            /// Performs an <c>as</c> operation.  This method should never
            /// throw an exception but may return null.  
            /// </summary>
            /// <param name="obj">
            /// Target object
            /// </param>
            /// <returns>
            /// <paramref name="obj"/> <c>as</c> <typeparamref name="T" />
            /// or <c>null</c>.
            /// </returns>
            T As<T>(object obj) where T : class;

            /// <summary>
            /// Performs an <c>is</c> check.  This method 
            /// should never throw an exception.
            /// </summary>
            /// <param name="obj">
            /// Target object
            /// </param>
            /// <returns>
            /// <c>True</c> if <paramref name="obj"/> can be cast to
            /// <typeparamref name="T"/>.  <c>False</c> otherwise.
            /// </returns>
            bool Is<T>(object obj);
        }

        /// <summary>
        /// Default <see cref="IAsIsImplementation"/>
        /// </summary>
        public class DefaultAsIsImplementation : IAsIsImplementation
        {
            /// <summary>
            /// Performs an <c>as</c> operation.  This method should never
            /// throw an exception but may return null.  
            /// </summary>
            /// <param name="obj">
            /// Target object
            /// </param>
            /// <returns>
            /// <paramref name="obj"/> <c>as</c> <typeparamref name="T" />
            /// or <c>null</c>.
            /// </returns>
            public virtual T As<T>(object obj)
                where T : class 
            {
                var objAsIWrap = obj as IContainMixin<T>;

                return (null != objAsIWrap)
                           ? objAsIWrap.MixinInstance
                           : obj as T;
            }

            /// <summary>
            /// Performs an <c>is</c> check.  This method 
            /// should never throw an exception.
            /// </summary>
            /// <param name="obj">
            /// Target object
            /// </param>
            /// <returns>
            /// <c>True</c> if <paramref name="obj"/> can be cast to
            /// <typeparamref name="T"/>.  <c>False</c> otherwise.
            /// </returns>
            public virtual bool Is<T>(object obj) 
            {
                var objAsIWrap = obj as IContainMixin<T>;

                return (null != objAsIWrap || obj is T);
            }
        }

        /// <summary>
        /// Function that returns an <see cref="IAsIsImplementation"/>.  By default, this will
        /// contain a function that returns a <see cref="DefaultAsIsImplementation"/>.
        /// </summary>
        /// <remarks>
        /// This function can be replaced with custom logic.  Be aware, that this function
        /// will be executed on every call to <see cref="As{T}"/> or <see cref="Is{T}"/>.
        /// </remarks>
        public static Func<IAsIsImplementation> AsIsImplementationFactory { get; set; }

        static AsIsHelper()
        {
            AsIsImplementationFactory = () => new DefaultAsIsImplementation();
        }

        /// <summary>
        /// Performs an <c>as</c> operation.  This method should never
        /// throw an exception but may return null.  
        /// </summary>
        /// <param name="obj">
        /// Target object
        /// </param>
        /// <returns>
        /// <paramref name="obj"/> <c>as</c> <typeparamref name="T" />
        /// or <c>null</c>.
        /// </returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return AsIsImplementationFactory().As<T>(obj);
        }

        /// <summary>
        /// Performs an <c>is</c> check.  This method 
        /// should never throw an exception.
        /// </summary>
        /// <param name="obj">
        /// Target object
        /// </param>
        /// <returns>
        /// <c>True</c> if <paramref name="obj"/> can be cast to
        /// <typeparamref name="T"/>.  <c>False</c> otherwise.
        /// </returns>
        public static bool Is<T>(this object obj)
        {
            return AsIsImplementationFactory().Is<T>(obj);
        }
    }
}
