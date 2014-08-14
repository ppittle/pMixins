//----------------------------------------------------------------------- 
// <copyright file="TypeInstanceActivator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 12:03:09 AM</date> 
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
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using log4net;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes
{
    public interface ITypeInstanceActivator
    {
        T CreateInstance<T>(IType type, params object[] constructorArguments) where T : class;

        bool TryCreateInstance<T>(IAttribute attribute, Action<T> onSuccess) where T : class;
    }

    public class TypeInstanceActivator : ITypeInstanceActivator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public object CreateInstance(Type type, params object[] constructorArguments)
        {
            Ensure.ArgumentNotNull(type, "type");

            return Activator.CreateInstance(type, constructorArguments);
        }

        public T CreateInstance<T>(Type type, params object[] constructorArguments)
        {
            Ensure.ArgumentNotNull(type, "type");

            return (T)CreateInstance(type, constructorArguments);
        }

        public Type LoadType(IType type)
        {
            Ensure.ArgumentNotNull(type, "type");

            var assembly = Assembly.Load(type.GetDefinition().ParentAssembly.FullAssemblyName);

            try
            {
                return assembly.GetType(type.GetFullTypeName(), true);
            }
            catch (TypeLoadException e)
            {
                Log.Warn("Could not load [" + type.FullName + "] from assembly [" + assembly.Location + "] because the type is not listed in the assembly!", e);
                throw;
            }
            catch (Exception e)
            {
                Log.Warn("Exception in LoadType(" + type.FullName + ")", e);
                throw;
            }
        }

        public T CreateInstance<T>(IType type, params object[] constructorArguments)
            where T : class
        {
            Ensure.ArgumentNotNull(type, "type");

            var castTypeDefinition = type as DefaultResolvedTypeDefinition;
            if (null == castTypeDefinition)
                throw new Exception("Unsure how to create an instance of a Resovled Type: " + type.GetType().Name);

            object instance;
            try
            {
                instance = CreateInstance(LoadType(type), constructorArguments);
            }
            catch (Exception e)
            {
                var wrappedException = new TargetInvocationException(string.Format(
                    "Exception Creating Instance of [{0}] from Assembly [{1}].  Has the Type been compiled by Visual Studio?: {2}",
                    castTypeDefinition.FullName,
                    castTypeDefinition.ParentAssembly.AssemblyName,
                    e.Message), e);

                Log.Error("Exception in CreateInstance [" + type.FullName + "]", wrappedException);
                throw wrappedException;
            }

            var castInstance = instance as T;

            if (null == castInstance)
                throw new Exception(string.Format(
                    "[{0}] does not inherit from {1}",
                    castTypeDefinition.FullName, typeof(T).FullName));

            return castInstance;
        }

        public bool TryCreateInstance<T>(IType type, Action<T> onSuccess, params object[] constructorArguments) where T : class
        {
            Ensure.ArgumentNotNull(type, "type");
            Ensure.ArgumentNotNull(onSuccess, "onSuccess");

            T instance;

            try
            {
                instance = CreateInstance<T>(type, constructorArguments);
            }
            catch (Exception e)
            {
                Log.Info("TryCreateInstance [" + type.FullName + "] failed.", e);
                return false;
            }

            onSuccess(instance);

            return true;
        }

        public T CreateInstance<T>(IAttribute attribute) where T : class
        {
            Ensure.ArgumentNotNull(attribute, "attribute");

            return CreateInstance<T>(attribute.AttributeType,
                                     attribute.PositionalArguments.Select(x => x.GetValue()).ToArray());
        }

        public bool TryCreateInstance<T>(IAttribute attribute, Action<T> onSuccess) where T : class
        {
            Ensure.ArgumentNotNull(attribute, "attribute");
            Ensure.ArgumentNotNull(onSuccess, "onSuccess");

            T instance;

            try
            {
                instance = CreateInstance<T>(attribute);
            }
            catch (Exception e)
            {
                Log.Info("TryCreateInstance [" + attribute.AttributeType.FullName + "] failed.", e);
                return false;
            }

            onSuccess(instance);

            return true;
        }
    }
}
