//----------------------------------------------------------------------- 
// <copyright file="CompilerResultsExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.Tests.Common.Extensions
{
    public static class CompilerResultsExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Load Types
        public static object TryLoadCompiledType(this CompilerResults compilerResults, string typeName, params object[] constructorArgs)
        {
            if (compilerResults.Errors.HasErrors)
            {
                Log.Warn("Can not TryLoadCompiledType because CompilerResults.HasErrors");
                return null;
            }

            var type = compilerResults.CompiledAssembly.GetType(typeName);

            if (null == type)
            {
                Log.Warn("Compiled Assembly does not contain a type [" + typeName + "]");
                return null;
            }

            return Activator.CreateInstance(type, constructorArgs);
        }

        public static T TryLoadCompiledType<T>(this CompilerResults compilerResults, params object[] constructorArgs)
            where T : class
        {
            var loadedType = compilerResults.TryLoadCompiledType(typeof(T).FullName, constructorArgs);

            return null == loadedType
                ? null
                : (T)loadedType;
        }
        #endregion

        #region Methods
        public static T ExecuteMethod<T>(this CompilerResults compilerResults, string typeName, string methodName,
            BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags, params object[] args)
        {
            return ExecuteMethod<T>(
                compilerResults,
                typeName,
                new object[0],
                methodName,
                bindingFlags,
                args);
        }

        public static T ExecuteMethod<T>(this CompilerResults compilerResults, 
            string typeName, object[] constructorArgs,
            string methodName, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags, params object[] args)
        {
            constructorArgs = constructorArgs ?? new object[0];

            return
                ReflectionHelper.ExecuteMethod<T>(
                    compilerResults.TryLoadCompiledType(typeName, constructorArgs),
                    methodName, bindingFlags, args);
        }

        public static void ExecuteVoidMethod(this CompilerResults compilerResults, string typeName, string methodName,
           BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags, params object[] args)
        {
            ExecuteVoidMethod(
                compilerResults,
                typeName,
                new object[0],
                methodName,
                bindingFlags,
                args);
        }

        public static void ExecuteVoidMethod(this CompilerResults compilerResults,
            string typeName, object[] constructorArgs,
            string methodName, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags, params object[] args)
        {
            constructorArgs = constructorArgs ?? new object[0];

            ReflectionHelper.ExecuteVoidMethod(
                    compilerResults.TryLoadCompiledType(typeName, constructorArgs),
                    methodName, bindingFlags, args);
        }

        public static string PrettyPrintErrorList(this CompilerResults compilerResults)
        {
            return
                string.Join(Environment.NewLine,
                    compilerResults.Errors
                        .Cast<CompilerError>()
                        .Where(e => !e.IsWarning)
                        .ToList()
                        .Select(e =>
                            string.Format("{0}{1}\t File: {2} - {3}",
                                e.ErrorText,
                                Environment.NewLine,
                                e.FileName,
                                e.Line)));
        }
        #endregion

        #region Properties

        public static T ExecutePropertyGet<T>(
            this CompilerResults compilerResults,
            string typeName,
            string propertyName, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags)
        {
            return ExecutePropertyGet<T>(
                 compilerResults,
                 typeName,
                 new object[0],
                 propertyName,
                 bindingFlags);
        }

        public static T ExecutePropertyGet<T>(
            this CompilerResults compilerResults,
            string typeName, object[] constructorArgs,
            string propertyName, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags)
        {
            constructorArgs = constructorArgs ?? new object[0];

            return
                ReflectionHelper.ExecutePropertyGet<T>(
                    compilerResults.TryLoadCompiledType(typeName, constructorArgs),
                    propertyName, bindingFlags);
        }

        public static void ExecutePropertySet<T>(
            this CompilerResults compilerResults,
            string typeName,
            string propertyName, T value, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags)
            where T : class
        {
            ExecutePropertySet(
                 compilerResults,
                 typeName,
                 new object[0],
                 propertyName,
                 value,
                 bindingFlags);
        }

        public static void ExecutePropertySet<T>(
            this CompilerResults compilerResults,
            string typeName, object[] constructorArgs,
            string propertyName, T value, BindingFlags bindingFlags = ReflectionHelper.DefaultBindingFlags)
            where T : class
        {
            constructorArgs = constructorArgs ?? new object[0];

            ReflectionHelper.ExecutePropertySet(
                    compilerResults.TryLoadCompiledType(typeName, constructorArgs),
                    propertyName, value, bindingFlags);
        }
        #endregion
    }

    public static class ReflectionHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const BindingFlags DefaultBindingFlags =
           BindingFlags.NonPublic |
           BindingFlags.Public |
           BindingFlags.Instance |
           BindingFlags.Static |
           BindingFlags.GetProperty |
           BindingFlags.SetProperty;

        #region Methods
        public static T ExecuteMethod<T>(object target, string methodName,
            BindingFlags bindingFlags = DefaultBindingFlags, params object[] args)
        {
            if (null == args)
                args = new object[0];

            if (args.Any(x => null == x))
                Assert.Fail("This version of ExecuteMethod can not accommodate invoking a method with a null argument.");

            var method = LoadMethod(target, methodName, bindingFlags, args.Select(x => x.GetType()).ToArray());
            
            try
            {
                var result = method.Invoke(target, args);

                if (null == result)
                    return default(T);

                if (!(result.GetType() == typeof(T)))
                    Assert.Fail(
                        "Expected result of call to [{0}.{1}({2})] to return a [{3}], but returned a [{4}]",
                        target.GetType().FullName,
                        methodName,
                        String.Join(",",
                            args.Select(x => (null == x) ? "null" : x.ToString())),
                        typeof(T).FullName,
                        result.GetType().FullName);

                return (T)result;
            }
            catch (Exception e)
            {
                var exceptionMessage =
                    String.Format(
                        "Exception calling [{0}.{1}({2})]: {3}",
                        target.GetType().FullName,
                        methodName,
                        String.Join(",",
                            args.Select(x => (null == x) ? "null" : x.ToString())),
                        e.Message);

                Log.Error(exceptionMessage, e);

                Assert.Fail(exceptionMessage);
            }
            return default(T);
        }

        public static void ExecuteVoidMethod(object target, string methodName,
            BindingFlags bindingFlags = DefaultBindingFlags, params object[] args)
        {
            var method = LoadMethod(target, methodName, bindingFlags);

            if (null == args)
                args = new object[0];
            try
            {
                method.Invoke(target, args);
            }
            catch (Exception e)
            {
                var exceptionMessage =
                    String.Format(
                        "Exception calling [{0}.{1}({2})]: {3}",
                        target.GetType().FullName,
                        methodName,
                        String.Join(",",
                            args.Select(x => (null == x) ? "null" : x.ToString())),
                        e.Message);

                Log.Error(exceptionMessage, e);

                Assert.Fail(exceptionMessage);
            }
        }

        [NotNull]
        private static MethodInfo LoadMethod(object target, string methodName, BindingFlags bindingFlags, Type[] types = null)
        {
            if (null == target)
                Assert.Fail("Couldn't load target type (target type is null");

            if (null == types)
                types = new Type[0];

            
            var method =
                target.GetType().GetMethod(methodName, bindingFlags, null, types, null);

            if (null == method)
                Assert.Fail("Couldn't find method [" + methodName + "] on Type [" + target.GetType().FullName + "]");

            return method;
        }
        #endregion

        #region Properties
        public static T ExecutePropertyGet<T>(object target, string propertyName, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var property = LoadProperty(target, propertyName, bindingFlags);

            if (!property.CanRead)
                Assert.Fail("Property [" + propertyName + "] on Type [" + target.GetType().FullName + "] can not read.");

            try
            {
                var result = property.GetValue(target);

                if (null == result)
                    return default(T);

                if (!(result.GetType() == typeof (T)))
                {
                    Assert.Fail(
                        "Expected result of Property [{0}.{1}] to return a [{2}], but returned a [{3}]",
                        target.GetType().FullName,
                        propertyName,
                        typeof (T).FullName,
                        result.GetType().FullName);
                }

                return (T)result;
            }
            catch (Exception e)
            {
                var exceptionMessage =
                    String.Format(
                        "Exception getting Property [{0}.{1}]: {2}",
                        target.GetType().FullName,
                        propertyName,
                        e.Message);

                Log.Error(exceptionMessage, e);

                Assert.Fail(exceptionMessage);                
            }
            return default(T);
        }

        public static void ExecutePropertySet<T>(object target, string propertyName, T value, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var property = LoadProperty(target, propertyName, bindingFlags);

            if (!property.CanWrite)
                Assert.Fail("Property [" + propertyName + "] on Type [" + target.GetType().FullName + "] can not write.");

            try
            {
                property.SetValue(target, value);
            }
            catch (Exception e)
            {
                var exceptionMessage =
                    String.Format(
                        "Exception setting Property [{0}.{1}] with object of type [{2}]: {3}",
                        target.GetType().FullName,
                        propertyName,
                        typeof(T).FullName,
                        e.Message);

                Log.Error(exceptionMessage, e);

                Assert.Fail(exceptionMessage);
            }
        }

        [NotNull]
        private static PropertyInfo LoadProperty(object target, string propertyName,
            BindingFlags bindingFlags = DefaultBindingFlags)
        {
            if (null == target)
                Assert.Fail("Couldn't load target type (target type is null");
            
            var property =
                target.GetType().GetProperty(propertyName, bindingFlags);

            if (null == property)
                Assert.Fail("Couldn't find Property [" + propertyName + "] on Type [" + target.GetType().FullName + "]");

            return property;
        }
        #endregion
    }
}
