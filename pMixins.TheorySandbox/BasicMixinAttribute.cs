//----------------------------------------------------------------------- 
// <copyright file="BasicMixinAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 22, 2013 10:04:30 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface,
        AllowMultiple = true, Inherited = false)]
    public class BasicMixinAttribute : Attribute
    {
        public BasicMixinAttribute()
        {
            Activator = typeof (DefaultMixinActivator);
        }

        public Type Target { get; set; }
        public Type Activator { get; set; }
        /// <summary>
        /// Allows the host to indicate they want to initialize the Mixin before it 
        /// is ever used.  Note:  This should be set to <c>true</c> automatically by the pipeline
        /// if the Mixin does not have a public parameterless constructor (or make an error and force user to set to true?)
        /// ----Wait, don't do that, if using a ServiceLocator activator, it might not be necessary?
        /// </summary>
        public bool RequiresInitialization { get; set; }

        /// <summary>
        /// Sets how much logging is done in the generated file.
        /// Default is <see cref="Verbosity.Error"/>
        /// </summary>
        public Verbosity LoggingInGenerateFileVerbosity { get; set; }

        /// <summary>
        /// Indicates if all loaded assemblies should be scanned for extension
        /// methods.  Extension methods will then be copied to work with the 
        /// host type.  Default is <c>False</c>.
        /// </summary>
        public bool GenerateExtensionMethodWrappers { get; set; }

        /// <summary>
        /// Collection of Interceptor types.  Interceptors can register
        /// for Join Point Events.  Interceptors must inherit from
        /// <see cref="IMixinInterceptor"/>
        /// </summary>
        public Type[] Interceptors { get; set; }

        /// <summary>
        /// When set to <c>True</c> abstract members are implemented
        /// in the target as abstract.
        /// 
        /// Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// It is an error to set this to <c>true</c> if the 
        /// target is not abstract.
        /// </remarks>
        public bool KeepAbstractMembersAbstract { get; set; }
    }

    public enum Verbosity
    {
        Error,
        Warning,
        Info,
        All
    }
}
