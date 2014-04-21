//----------------------------------------------------------------------- 
// <copyright file="VisualStudioEventProxyFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, March 17, 2014 10:59:23 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using EnvDTE80;

namespace CopaceticSoftware.pMixins_VSPackage.Infrastructure
{
    /// <summary>
    /// Lazy thread safe loading of <see cref="IVisualStudioEventProxy"/>
    /// </summary>
    public class VisualStudioEventProxyFactory 
    {
        private static readonly object _lock = new object();

        private static IVisualStudioEventProxy _visualStudioEventProxy;

        public IVisualStudioEventProxy BuildVisualStudioEventProxy(Func<DTE2> deferredVisualStudioLoader)
        {
            if (null == _visualStudioEventProxy)
            {
                lock (_lock)
                {
                    if (null == _visualStudioEventProxy)
                    {
                        _visualStudioEventProxy = new VisualStudioEventProxy(deferredVisualStudioLoader());
                    }
                }
            }

            return _visualStudioEventProxy;
        }
    }
}
