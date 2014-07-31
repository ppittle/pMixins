﻿//----------------------------------------------------------------------- 
// <copyright file="MixinLevelCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 1:38:56 PM</date> 
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

using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.CreateTypeDeclarations;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator
{
    /// <summary>
    /// Controller class for executing the Mixin Level Code Generator Pipeline.
    /// </summary>
    public class MixinLevelCodeGenerator : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        private readonly IPipelineStep<MixinLevelCodeGeneratorPipelineState>[] _createTypeDeclarations =
            { 
                new CreateExternalMixinSpecificAutoGeneratedNamespace(),
                new CreateMixinLevelAutoGeneratedContainerClass(),  
                new CreateMixinRequirementsInterfaceTypeDeclaration(), 
                new CreateProtectedWrapperTypeDeclaration(), 
                new CreateAbstractWrapperTypeDeclaration(), 
                new CreateMasterWrapperTypeDeclaration() 
            };

        private readonly IPipelineStep<MixinLevelCodeGeneratorPipelineState>[] _generateMembers =
            { 
                new GenerateRequiredInterfaceMembers(), 
                new GenerateProtectedWrapperMembers(),  
                new GenerateAbstractWrapperMembers(),  
                new GenerateMasterWrapperConstructor(), 
                new GenerateMasterWrapperVirtualAndDataMemberInstances(), 
                new GenerateMasterWrapperActivateDependencyMethod(), 
                new GenerateMasterWrapperMembers(), 
                new GenerateMembersInTargetClass()
            };

        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            if (!_createTypeDeclarations.RunPipeline(manager,
                haltOnStepFailing: step => true))
            {
                return false;
            }

            if (!_generateMembers.RunPipeline(manager,
                haltOnStepFailing: step => true))
            {
                return false;
            }

            return true;
        }
    }
}
