//----------------------------------------------------------------------- 
// <copyright file="IResolveMembersPipelineState.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 10:15:58 AM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveMembers
{
    public interface IResolveMembersPipelineState 
    {
        /// <summary>
        /// The State from the previous Resolve Attributes
        /// step.
        /// </summary>
        IResolveAttributesPipelineState ResolveAttributesPipeline { get; }

        /// <summary>
        /// Dictionary of <see cref="MemberWrapper"/> representing
        /// each <see cref="IPipelineCommonState.SourcePartialClassDefinitions"/>'s
        /// <see cref="IPMixinAttribute"/>'s Members.
        /// </summary>
        Dictionary<TypeDeclaration, IList<MemberWrapper>> MixinMembers { get; }
    }
}
