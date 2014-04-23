//----------------------------------------------------------------------- 
// <copyright file="MasterWrapperBase.cs" company="Copacetic Software"> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox
{
    public abstract class MasterWrapperBase
    {
        private object _target;
        private object _mixin;

        protected IEnumerable<IMixinInterceptor> Interceptors { get; private set; }

        protected void Initialize(object target, object mixin, IEnumerable<IMixinInterceptor> interceptors)
        {
            _target = target;
            _mixin = mixin;

            Interceptors = interceptors;

            //Fire Initialized Event
            foreach (var interceptor in Interceptors)
                interceptor.OnMixinInitialized(
                    this,
                    new InterceptionEventArgs
                    {
                        Target = _target,
                        Mixin = _mixin
                    });
        }

        protected TReturnType ExecuteMethod<TReturnType>(
            string methodName,
            IEnumerable<Parameter> parameters,
            Func<TReturnType> methodInvocation)
        {
            TReturnType returnValue = default(TReturnType);

            ExecuteMethodImplementation(
                typeof(TReturnType),
                methodName,
                parameters,
                methodEventArgs => methodEventArgs.ReturnValue = methodInvocation(),
                invocationComplete: methodEventArgs =>
                    {
                        returnValue =
                                (TReturnType) 
                                ((null != methodEventArgs.CancellationToken && methodEventArgs.CancellationToken.Cancel)
                                    ? methodEventArgs.CancellationToken.ReturnValue
                                    : methodEventArgs.ReturnValue);
                    }
                );

            return returnValue;
        }

        protected void ExecuteVoidMethod(
            string methodName,
            IEnumerable<Parameter> parameters,
            Action methodInvocation)
        {
            ExecuteMethodImplementation(
                typeof(void),
                methodName,
                parameters,
                methodEventArgs => methodInvocation());
        }

        private void ExecuteMethodImplementation(
            Type returnType,
            string methodName,
            IEnumerable<Parameter> parameters,
            Action<MemberEventArgs> callMethodInvocationDelegate,
            Action<MemberEventArgs> invocationComplete = null)
        {
            var eventArgs =
                            new MethodEventArgs
                            {
                                Target = _target,
                                Mixin = _mixin,
                                MemberName = methodName,
                                ReturnType = returnType,
                                Parameters = parameters
                            };


            foreach (var interceptor in Interceptors)
            {
                interceptor.OnBeforeMethodInvocation(
                    this, eventArgs);

                if (null != eventArgs.CancellationToken && eventArgs.CancellationToken.Cancel)
                {
                    if (null != invocationComplete)
                        invocationComplete(eventArgs);
                    
                    return;
                }
            }

            callMethodInvocationDelegate(eventArgs);

            foreach (var interceptor in Interceptors)
            {
                interceptor.OnAfterMethodInvocation(
                    this, eventArgs);

                if (null != eventArgs.CancellationToken && eventArgs.CancellationToken.Cancel)
                {
                    if (null != invocationComplete)
                        invocationComplete(eventArgs);
                    

                    return;
                }
            }

            if (null != invocationComplete)
                invocationComplete(eventArgs);

        }

        protected TReturnType ExecutePropertyGet<TReturnType>
            (string propertyName,
                Func<TReturnType> methodInvocation)
        {
            TReturnType returnValue = default (TReturnType);

            ExecutePropertyImplementation<TReturnType>(
                propertyName,
                new List<Parameter>(),
                eventArgs => eventArgs.ReturnValue = methodInvocation(),
                invocationComplete: methodEventArgs =>
                {
                    returnValue =
                            (TReturnType)
                            ((null != methodEventArgs.CancellationToken && methodEventArgs.CancellationToken.Cancel)
                                ? methodEventArgs.CancellationToken.ReturnValue
                                : methodEventArgs.ReturnValue);
                });

            return returnValue;
        }

        protected void ExecutePropertySet<TReturnType>(
            string propertyName,
            TReturnType value,
            Action<TReturnType> methodInvocation)
        {
            ExecutePropertyImplementation<TReturnType>(
                propertyName,
                new List<Parameter>
                {
                    new Parameter{Name="value", Type = typeof(TReturnType), Value = value}
                },
                eventArgs => methodInvocation(value));
        }

        private void ExecutePropertyImplementation<TReturnType>(
            string propertyName,
            IEnumerable<Parameter> parameters,
            Action<MemberEventArgs> callMethodInvocationDelegate,
            Action<MemberEventArgs> invocationComplete = null)
        {
            var eventArgs =
                            new PropertyEventArgs
                            {
                                Target = _target,
                                Mixin = _mixin,
                                MemberName = propertyName,
                                ReturnType = typeof(TReturnType),
                                Parameters = parameters
                            };


            foreach (var interceptor in Interceptors)
            {
                interceptor.OnBeforePropertyInvocation(
                    this, eventArgs);

                if (null != eventArgs.CancellationToken && eventArgs.CancellationToken.Cancel)
                {
                    if (null != invocationComplete)
                        invocationComplete(eventArgs);
                    
                    return;
                }
            }

            callMethodInvocationDelegate(eventArgs);

            foreach (var interceptor in Interceptors)
            {
                interceptor.OnAfterPropertyInvocation(
                    this, eventArgs);

                if (null != eventArgs.CancellationToken && eventArgs.CancellationToken.Cancel)
                {
                    if (null != invocationComplete)
                        invocationComplete(eventArgs);

                    return;
                }
            }

            if (null != invocationComplete)
                invocationComplete(eventArgs);

        }
    }
}
