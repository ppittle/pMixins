//----------------------------------------------------------------------- 
// <copyright file="IGenerateCodePipelineState.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveMembers;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind
{
    public interface IGenerateCodePipelineState
    {
        IPipelineCommonState CommonState { get; }

        /// <summary>
        /// The State from the previous Resolve Members
        /// step.
        /// </summary>
        IResolveMembersPipelineState ResolveMembersPipeline { get; }

        /// <summary>
        /// Dictionary of <see cref="MemberWrapper"/> representing
        /// each <see cref="IPipelineCommonState.SourcePartialClassDefinitions"/>'s
        /// <see cref="IPMixinAttribute"/>'s Members.
        /// </summary>
        //Dictionary<TypeDeclaration, IList<MemberWrapper>> MixinMembers { get; }

        //TypeDeclaration AbstractMembersWrapper { get; set; }

        SyntaxTree CodeBehindSyntaxTree { get; }
    }
}
