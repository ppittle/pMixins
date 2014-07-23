//----------------------------------------------------------------------- 
// <copyright file="TargetLevelCodeGeneratorPipeline.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 11:34:24 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveMembers;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator
{
    public class TargetLevelCodeGeneratorPipeline : IGenerateCodePipelineState
    {
        public TargetLevelCodeGeneratorPipeline(IGenerateCodePipelineState baseState)
        {
            CommonState = baseState.CommonState;
            ResolveMembersPipeline = baseState.ResolveMembersPipeline;
            CodeBehindSyntaxTree = baseState.CodeBehindSyntaxTree;
        }

        #region IGenerateCodePipelineState

        public IPipelineCommonState CommonState { get; private set; }
        public IResolveMembersPipelineState ResolveMembersPipeline { get; private set; }
        
        //TypeDeclaration AbstractMembersWrapper { get; set; }
        public SyntaxTree CodeBehindSyntaxTree { get; private set; }

        #endregion

        public TypeDeclaration TargetSourceTypeDeclaration { get; set; }
        public TypeDeclaration TargetCodeBehindTypeDeclaration { get; set; }
    }
}
