//----------------------------------------------------------------------- 
// <copyright file="pMixinAttributeResolvedResult.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure
{
    /// <summary>
    /// Represents a resolved <see cref="pMixinAttribute"/> and has a 
    /// copy of every property <see cref="pMixinAttribute"/> has.
    /// </summary>
    /// <remarks>
    /// Because a <see cref="pMixinAttribute"/> can be written in uncompiled
    /// code and, more importantly, it can reference uncompiled types, it is
    /// not possible to simply create a new instance of a <see cref="pMixinAttribute"/>. 
    /// </remarks>
    public class pMixinAttributeResolvedResult : pMixinAttributeResolvedResultBase
    {
        /// <summary>
        /// Creates a new <see cref="pMixinAttributeResolvedResult"/>
        /// </summary>
        /// <param name="attribute">
        /// Reference to the <see cref="CopaceticSoftware.pMixin"/>'s source code.
        /// </param>
        public pMixinAttributeResolvedResult(IAttribute attribute)
            : base(attribute)
        {
            Masks = new List<IType>(0);
            Interceptors = new List<IType>(0);
            EnableSharedRequirementsInterface = new pMixinAttribute().EnableSharedRequirementsInterface;
        }

        /// <summary>
        /// Matches <see cref="pMixinAttribute.Mixin"/>
        /// </summary>
        public IType Mixin { get; set; }

        /// <summary>
        /// Matches <see cref="pMixinAttribute.Masks"/>
        /// </summary>
        public IList<IType> Masks { get; set; }

        /// <summary>
        /// Matches <see cref="pMixinAttribute.Interceptors"/>
        /// </summary>
        public IList<IType> Interceptors { get; set; }
        
        /// <summary>
        /// Matches <see cref="pMixinAttribute.ExplicitlyInitializeMixin"/>
        /// </summary>
        public bool ExplicitlyInitializeMixin { get; set; }

        /// <summary>
        /// Matches <see cref="pMixinAttribute.EnableSharedRequirementsInterface"/>
        /// </summary>
        public bool EnableSharedRequirementsInterface { get; set; }
    }
}
