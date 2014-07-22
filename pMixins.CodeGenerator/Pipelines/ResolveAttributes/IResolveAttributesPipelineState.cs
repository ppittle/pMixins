//----------------------------------------------------------------------- 
// <copyright file="IResovleAttributesPipelineState.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes
{
    public interface IResolveAttributesPipelineState : IParseSourceFilePipelineState
    {
        ITypeInstanceActivator TypeInstanceActivator { get; }

        /// <summary>
        /// Dictionary of <see cref="pMixinAttributeResolvedResultBase"/> representing
        /// the <see cref="IPMixinAttribute"/>s parsed in the source, and mapped to each
        /// parsed class.
        /// </summary>
        Dictionary<TypeDeclaration, IList<pMixinAttributeResolvedResultBase>> 
            PartialClassLevelResolvedPMixinAttributes { get; }
    }
}
