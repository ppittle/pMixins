//----------------------------------------------------------------------- 
// <copyright file="IMixinInterceptor.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 20, 2014 11:12:49 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox
{
    public interface IMixinInterceptor
    {
        void OnMixinInitialized(object sender, InterceptionEventArgs args);
        void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs);

        void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs);


        void OnBeforePropertyInvocation(object sender, PropertyEventArgs eventArgs);
        void OnAfterPropertyInvocation(object sender, PropertyEventArgs eventArgs);
    }

    public abstract class MixinInterceptorBase : IMixinInterceptor
    {
        public virtual void OnMixinInitialized(object sender, InterceptionEventArgs args){}

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

        public Exception MemberInvocationException { get; set;  }
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
