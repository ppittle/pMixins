//----------------------------------------------------------------------- 
// <copyright file="MasterWrapperBase.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.Interceptors;

namespace CopaceticSoftware.pMixins.Infrastructure
{
    /// <summary>
    /// Provides the infrastructure for proxying Mixed in type's member
    /// invocation events to <see cref="IMixinInterceptor"/>s.  Generated 
    /// MasterWrapper's inherit from this base class.
    /// </summary>
    /// <remarks>
    /// This class is intended for internal use by pMixin
    /// and should not be used directly.
    /// </remarks>
    public abstract class MasterWrapperBase
    {
        private object _target;
        private object _mixin;

        protected IEnumerable<IMixinInterceptor> Interceptors { get; private set; }

        protected MasterWrapperBase()
        {
            Interceptors = Enumerable.Empty<IMixinInterceptor>();
        }

        #region Mixin Activator

        protected TMixin TryActivateMixin<TMixin>(params object[] p)
        {
            try
            {
                return MixinActivatorFactory.GetCurrentActivator()
                    .CreateInstance<TMixin>(p);
            }
            catch (Exception e)
            {
                throw new MixinActivationException(
                    string.Format(
                    @"Could not create Mixin of Type [{0}] with Params [{1}]: {2}.  
                            
                      Did you mean to use 'ExplicitlyInitializeMixin = true' on the pMixin Attribute constructor?",
                        typeof(TMixin).FullName,
                        string.Join(",",
                            (p ?? new object[0])
                            .Select(o => 
                                (o == null)
                                ? "null"
                                : o.GetType().FullName)
                            .ToArray()),
                        e.Message),
                    e);
            }
        }

        #endregion

        #region Interceptors

        /// <summary>
        /// Called during a Master Wrapper's constructor
        /// to initialize the <see cref="IMixinInterceptor"/> proxying
        /// infrastructure.
        /// </summary>
        /// <param name="target">
        /// The Target that of the Mixin
        /// </param>
        /// <param name="mixin">
        /// The Mixed in instance
        /// </param>
        /// <param name="interceptors">
        /// Optional array of <see cref="IMixinInterceptor"/>s
        /// </param>
        protected void Initialize(object target, object mixin, 
            IEnumerable<IMixinInterceptor> interceptors = null)
        {
            _target = target;
            _mixin = mixin;

            Interceptors = interceptors ?? new IMixinInterceptor[0];

            //Fire Initialized Event
            foreach (var interceptor in Interceptors)
                interceptor.OnTargetInitialized(
                    this,
                    new InterceptionEventArgs
                    {
                        Target = _target,
                        Mixin = _mixin
                    });
        }

        /// <summary>
        /// Proxies a non-void method invocation.
        /// </summary>
        /// <typeparam name="TReturnType">
        /// Method return type.
        /// </typeparam>
        /// <param name="methodName">
        /// Method name that will be invoked.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="parameters">
        /// Parameter list.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="methodInvocation">
        /// Function for invoking the method.
        /// </param>
        /// <remarks>
        /// A <see cref="IMixinInterceptor"/> can prevent the 
        /// method from being invoked 
        /// <paramref name="methodInvocation"/> from firing or 
        /// could change the return value after it has been invoked.
        /// </remarks>
        /// <returns>
        /// The result of <paramref name="methodInvocation"/> or
        /// the result returned from an <see cref="IMixinInterceptor"/>.
        /// </returns>
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

        /// <summary>
        /// Proxies a void method invocation.
        /// </summary>
        /// <param name="methodName">
        /// Method name that will be invoked.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="parameters">
        /// Parameter list.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="methodInvocation">
        /// Action for invoking the method.
        /// </param>
        /// <remarks>
        /// A <see cref="IMixinInterceptor"/> can prevent the 
        /// method from being invoked 
        /// <paramref name="methodInvocation"/> from firing or 
        /// could change the return value after it has been invoked.
        /// </remarks>
        /// <returns>
        /// The result of <paramref name="methodInvocation"/> or
        /// the result returned from an <see cref="IMixinInterceptor"/>.
        /// </returns>
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
            var eventArgs = new MethodEventArgs
                            {
                                Target = _target,
                                Mixin = _mixin,
                                MemberName = methodName,
                                ReturnType = returnType,
                                Parameters = parameters
                            };

            if (FireInterceptorEvent(
                    eventArgs,
                    (i, e) => i.OnBeforeMethodInvocation(this, eventArgs),
                    invocationComplete))
            {
                return;
            }

            try
            {
                callMethodInvocationDelegate(eventArgs);
            }
            catch (Exception exc)
            {
                eventArgs.MemberInvocationException = exc;

                if (FireInterceptorEvent(
                    eventArgs,
                    (i, e) => i.OnAfterMethodInvocation(this, eventArgs),
                    invocationComplete))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }

            FireInterceptorEvent(
                eventArgs,
                (i, e) => i.OnAfterMethodInvocation(this, eventArgs),
                invocationComplete);
            
            if (null != invocationComplete)
                invocationComplete(eventArgs);

        }

        /// <summary>
        /// Fires the event and returns true if cancellation token is present.
        /// </summary>
        private bool FireInterceptorEvent<TEventArgs>(
            TEventArgs eventArgs,
            Action<IMixinInterceptor, TEventArgs> interceptorEvent, 
            Action<TEventArgs> invocationComplete)
            where TEventArgs : MemberEventArgs
        {
            foreach (var interceptor in Interceptors)
            {
                interceptorEvent(interceptor, eventArgs);

                if (null != eventArgs.CancellationToken && eventArgs.CancellationToken.Cancel)
                {
                    if (null != invocationComplete)
                        invocationComplete(eventArgs);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Proxies a Get Property invocation.
        /// </summary>
        /// <typeparam name="TReturnType">
        /// Property return type.
        /// </typeparam>
        /// <param name="propertyName">
        /// Name of the Property that will be invoked.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="methodInvocation">
        /// Function for invoking the method.
        /// </param>
        /// <remarks>
        /// A <see cref="IMixinInterceptor"/> can prevent the 
        /// method from being invoked 
        /// <paramref name="methodInvocation"/> from firing or 
        /// could change the return value after it has been invoked.
        /// </remarks>
        /// <returns>
        /// The result of <paramref name="methodInvocation"/> or
        /// the result returned from an <see cref="IMixinInterceptor"/>.
        /// </returns>
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

        /// <summary>
        /// Proxies a Set Property invocation.
        /// </summary>
        /// <typeparam name="TReturnType">
        /// Property return type.
        /// </typeparam>
        /// <param name="propertyName">
        /// Name of the Property that will be invoked.  This will
        /// be passed to the <see cref="IMixinInterceptor"/>s but
        /// does not influence method invocation.
        /// </param>
        /// <param name="value"/>The <c>value</c> passed into 
        /// the Property by the invoker.
        /// <param name="methodInvocation">
        /// Function for invoking the method.
        /// </param>
        /// <remarks>
        /// A <see cref="IMixinInterceptor"/> can prevent the 
        /// method from being invoked 
        /// <paramref name="methodInvocation"/> from firing or 
        /// could change the return value after it has been invoked.
        /// </remarks>
        /// <returns>
        /// The result of <paramref name="methodInvocation"/> or
        /// the result returned from an <see cref="IMixinInterceptor"/>.
        /// </returns>
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
            var eventArgs = new PropertyEventArgs
                            {
                                Target = _target,
                                Mixin = _mixin,
                                MemberName = propertyName,
                                ReturnType = typeof(TReturnType),
                                Parameters = parameters
                            };

            if (FireInterceptorEvent(
                    eventArgs,
                    (i, e) => i.OnBeforePropertyInvocation(this, eventArgs),
                    invocationComplete))
            {
                return;
            }


            try
            {
                callMethodInvocationDelegate(eventArgs);
            }
            catch (Exception exc)
            {
                eventArgs.MemberInvocationException = exc;

                if (FireInterceptorEvent(
                    eventArgs,
                    (i, e) => i.OnAfterPropertyInvocation(this, eventArgs),
                    invocationComplete))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }

            FireInterceptorEvent(
                eventArgs,
                (i, e) => i.OnAfterPropertyInvocation(this, eventArgs),
                invocationComplete);

            if (null != invocationComplete)
                invocationComplete(eventArgs);

        }

        #endregion
    }

    [Serializable]
    public class MixinActivationException : Exception
    {
        public MixinActivationException() : base() { }
        public MixinActivationException(string message) : base(message) { }
        public MixinActivationException(string message, Exception e) : base(message, e) { }
    }
}
