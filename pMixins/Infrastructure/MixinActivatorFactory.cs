//----------------------------------------------------------------------- 
// <copyright file="MixinActivatorFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, February 26, 2014 3:23:19 PM</date> 
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

namespace CopaceticSoftware.pMixins.Infrastructure
{
    /// <summary>
    /// Singleton class responsible for creating and managing access
    /// to the <see cref="IMixinActivator"/>, which is used to 
    /// create objects in the pMixin infrastructure.
    /// </summary>
    /// <remarks>
    /// Dependency injection can be introduced by creating a custom
    /// <see cref="IMixinActivator"/> and registering it with
    /// <see cref="SetCurrentActivator"/>.
    /// </remarks>
    public class MixinActivatorFactory
    {
        private static readonly MixinActivatorFactory Instance = new MixinActivatorFactory();

        private IMixinActivator _mixinActivator = new DefaultMixinActivator();
        
        /// <summary>
        /// Singleton
        /// </summary>
        private MixinActivatorFactory(){}

        public static IMixinActivator GetCurrentActivator()
        {
            return Instance._mixinActivator;
        }

        public static void SetCurrentActivator(IMixinActivator activator)
        {
            if (null == activator)
                throw new ArgumentNullException("activator");

            Instance._mixinActivator = activator;
        }
    }
}
