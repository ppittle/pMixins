﻿//----------------------------------------------------------------------- 
// <copyright file="pMixinPartialCodeGeneratorPipeline.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, January 20, 2014 11:13:19 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ValidateSourceFile;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines
{
    public class pMixinPartialCodeGeneratorPipeline : IPipelineStep<CreateCodeGenerationPipelineState>
    {
        private readonly IPipelineStep<IParseSourceFilePipelineState>[] _parseSourcePipeline =
        {
            new ParseSourceClassDefinitions(),
            new ParsepMixinAttributes()
        };

        private readonly IPipelineStep<IParseSourceFilePipelineState>[] _validateSourcePipeline =
        {
            new StopIfDisableCodeGenerationAttributeIsPresentInAssembly(),
            new PrunePartialClassDefinitionsDecoratedWithDisableCodeGeneratorAttribute(),
            new StopIfSourceCodeDoesNotHaveAPartialClassDefinition(),
            new WarnIfMixinAttributeIsOnANonPartialClass(),
            new WarnIfNoMixinAttributeInSourceFile()
        };

        private readonly IPipelineStep<IResolveAttributesPipelineState>[] _resolveAttributesPipeline =
        {
            new InitializeResolveAttributesPipeline(),
            new ResolveAttributes.Steps.ResolveAttributes()
        };

        /*
       private readonly IPipelineStep<ICodeGenerationPipelineState>[] _codeGenerationPipeline =
           {
               new AutoGeneratedCommentMessage(),
               new pMixinGenerator()
           };
        */

        private readonly IPipelineStep<ICreateCodeGenerationPlanPipelineState>[] _createCodeGenerationPlanPipeline =
        {
            new CreateTargetSpecificCodeGenerationPlans(),
            new CollectAllMembers(),
            new SetMemberImplementationDetails(), 
            new SetSharedRequirementsInterfacePlan(), 
            new SetMixinGenerationPlanDetails(), 
            new CreateProtectedWrapperPlan(),
            new CreateAbstractWrapperPlan(), 
            new CreateMasterWrapperPlan(), 
            new FilterMixinMembersForPromotionToTarget(), 
            new CalculateTargetSpecificImplicitConversionTypes(),
            new CalculateTargetSpecificMixinInterfacesToImplement(),
            new CalculateMixinAttributesForTargetCodeBehind()
        };

        private readonly IPipelineStep<IGenerateCodePipelineState>[] _generateCodeBehind =
            {
                new AddCodeBehindHeaderCommentMessage(),
                new RunTargetLevelCodeGeneratorForEachTarget()
            };       

        public bool PerformTask(CreateCodeGenerationPipelineState manager)
        {
             if (!_parseSourcePipeline.RunPipeline(manager,
                    haltOnStepFailing: step => true))
             {
                 return false;
             }

             if (!_validateSourcePipeline.RunPipeline(manager,
                     haltOnStepFailing: step => true))
             {
                 return false;
             }

             if (!_resolveAttributesPipeline.RunPipeline(manager,
                      haltOnStepFailing: step => true))
             {
                 return false;
             }

             if (!_createCodeGenerationPlanPipeline.RunPipeline(manager,
                       haltOnStepFailing: step => true))
             {
                 return false;
             }

            /*
             if (!_codeGenerationPipeline.RunPipeline(manager,
                      haltOnStepFailing: step => true))
             {
                 return false;
             }
             */

             if (!_generateCodeBehind.RunPipeline(manager,
                       haltOnStepFailing: step => true))
             {
                 return false;
             }


            return true;
        }
    }
}
