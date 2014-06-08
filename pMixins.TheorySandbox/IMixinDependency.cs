//----------------------------------------------------------------------- 
// <copyright file="IMixinDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, June 08, 2014 9:38:58 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox
{
    public interface IMixinDependency<T> where T : class
    {
        /// <summary>
        /// Property will be injected by the pMixin infrastructure.
        /// Do NOT use in Constructor, use the event <see cref="OnDependencySet"/>
        /// </summary>
        [DoNotMixin]
        T Dependency { get; set; }

        /// <summary>
        /// Called after the <see cref="Dependency"/>
        /// is set.
        /// </summary>
        [DoNotMixin]
        void OnDependencySet();
    }
}
