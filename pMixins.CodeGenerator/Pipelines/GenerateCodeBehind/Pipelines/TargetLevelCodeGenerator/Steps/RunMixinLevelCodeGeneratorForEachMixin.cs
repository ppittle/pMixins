//----------------------------------------------------------------------- 
// <copyright file="RunMixinLevelCodeGeneratorForEachMixin.cs" company="Copacetic Software"> 
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

using System.Linq;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps
{
    /// <summary>
    /// Iterate through each <see cref="IPipelineCommonState.SourcePartialClassDefinitions"/>
    /// and launch the <see cref="TargetLevelCodeGenerator"/>.
    /// </summary>
    public class RunMixinLevelCodeGeneratorForEachMixin : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var targetsMixins =
                manager
                    .CreateCodeGenerationPlanPipeline
                    .ResolveAttributesPipeline
                    .PartialClassLevelResolvedPMixinAttributes[manager.TargetSourceTypeDeclaration]
                    .OfType<pMixinAttributeResolvedResult>();

            foreach (var mixin in targetsMixins)
            {
                var mixinLevelCodeGeneratorPipeline =
                    new MixinLevelCodeGeneratorPipelineState(manager)
                    {
                        TargetLevelCodeGeneratorPipelineState = manager,
                        MixinGenerationPlan = manager.CodeGenerationPlan.MixinGenerationPlans[mixin]
                    };

                new MixinLevelCodeGenerator.MixinLevelCodeGenerator().PerformTask(
                    mixinLevelCodeGeneratorPipeline);
            }

            return true;
        }
    }
}
