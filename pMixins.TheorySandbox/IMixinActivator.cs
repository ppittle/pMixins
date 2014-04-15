//----------------------------------------------------------------------- 
// <copyright file="IMixinActivator.cs" company="Copacetic Software"> 
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
    public interface IMixinActivator
    {
        T CreateInstance<T>(params object[] constructorArgs);
    }

    public class DefaultMixinActivator : IMixinActivator
    {
        public T CreateInstance<T>(params object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(typeof(T), constructorArgs);
        }
    }
}
