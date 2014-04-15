//----------------------------------------------------------------------- 
// <copyright file="IMixinInterceptor.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, February 24, 2014 1:26:29 PM</date> 
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
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.Interceptors
{
    /// <summary>
    /// Interface for Mixin Interceptors, which can 
    /// receive member invocation events to manipulate if 
    /// the member should be invoked, or manipulate the return value.
    /// </summary>
    /// <remarks>
    /// If the <see cref="IMixinInterceptor"/> needs additional 
    /// classes to be Mixed in, it can be decorated with the 
    /// <see cref="InterceptorMixinRequirementAttribute"/>
    /// </remarks>
    /// <example>
    /// See <see cref="MixinInterceptorBase"/> for an example.
    /// </example> 
    public interface IMixinInterceptor
    {
        void OnMixinInitialized(object sender, InterceptionEventArgs args);

        void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs);
        void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs);
        
        void OnBeforePropertyInvocation(object sender, PropertyEventArgs eventArgs);
        void OnAfterPropertyInvocation(object sender, PropertyEventArgs eventArgs);
    }

    /// <summary>
    /// Convenience base class for <see cref="IMixinInterceptor"/> that
    /// provides empty event handlers for <see cref="IMixinInterceptor"/> 
    /// events.  Inheritors can override only the methods they need.
    /// </summary>
    /// <example>
    /// This example shows the pseudo code for a 
    /// Caching Interceptor, which will return a value
    /// from cache if it exists instead of allowing the 
    /// method invocation to proceed.
    /// <code>
    /// <![CDATA[
    /// public class CacheInterceptor : MixinInterceptorBase
    ///   {
    ///       public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
    ///       {
    ///           //Check if we have a cached copy of this method's return value
    ///
    ///           object objectInCache;
    ///
    ///           if (TryLoadFromCache(eventArgs, out objectInCache))
    ///               eventArgs.CancellationToken = new CancellationToken
    ///               {
    ///                   Cancel = true,
    ///                   ReturnValue = objectInCache
    ///               };
    ///       }
    ///
    ///       public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
    ///       {
    ///           //Save the result of this method in cache. 
    ///
    ///           AddToCache(eventArgs, eventArgs.ReturnValue);
    ///       }
    ///
    ///       private bool TryLoadFromCache(MethodEventArgs eventArgs, out object objectInCache)
    ///       {
    ///           //implementation excluded
    ///       }
    ///
    ///       private void AddToCache(MethodEventArgs eventArgs, object returnValue)
    ///       {
    ///           //implementation excluded
    ///       }
    ///   }
    /// ]]>
    /// </code>
    /// </example>
    public abstract class MixinInterceptorBase : IMixinInterceptor
    {
        public virtual void OnMixinInitialized(object sender, InterceptionEventArgs args) { }

        public virtual void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs) { }

        public virtual void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs) { }

        public virtual void OnBeforePropertyInvocation(object sender, PropertyEventArgs eventArgs) { }

        public virtual void OnAfterPropertyInvocation(object sender, PropertyEventArgs eventArgs) { }
    }


    public class InterceptionEventArgs
    {
        public object Target { get; set; }
        public object Mixin { get; set; }
    }

    public abstract class MemberEventArgs : InterceptionEventArgs
    {
        public string MemberName { get; set; }
        public System.Type ReturnType { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public object ReturnValue { get; set; }

        public Exception MemberInvocationException { get; set; }
    }

    public class MethodEventArgs : MemberEventArgs { }

    public class PropertyEventArgs : MemberEventArgs
    {
        public bool IsGet
        {
            get { return (null != base.Parameters && base.Parameters.Any()); }
        }

        public bool IsSet
        {
            get { return !IsGet; }
        }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public System.Type Type { get; set; }
        public object Value { get; set; }
    }

    public class CancellationToken
    {
        public bool Cancel { get; set; }
        public object ReturnValue { get; set; }
    }
}
