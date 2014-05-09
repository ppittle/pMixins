//----------------------------------------------------------------------- 
// <copyright file="pMixinAttributeResolvedResultBase.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure
{
    /// <summary>
    /// Base class for a resolved result for ANY <see cref="IPMixinAttribute"/>,
    /// NOT just <see cref="pMixinAttribute"/>
    /// </summary>
    public abstract class pMixinAttributeResolvedResultBase
    {
        /// <summary>
        /// Gets a reference to the Attribute's source.
        /// </summary>
        public IAttribute AttributeDefinition { get; private set; }

        protected pMixinAttributeResolvedResultBase(IAttribute attribute)
        {
            AttributeDefinition = attribute;
        }
    }
}
