//----------------------------------------------------------------------- 
// <copyright file="CreateTagetSpecificCodeGenerationPlans.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 3:54:20 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    public class CreateTargetSpecificCodeGenerationPlans : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var target in manager.CommonState.SourcePartialClassDefinitions)
            {
                var codeGenerationPlan = new CodeGenerationPlan
                {
                    SourceClass = target
                };

                foreach (var mixin in manager.GetAllPMixinAttributes(target))
                    codeGenerationPlan.MixinGenerationPlans.Add(mixin, new MixinGenerationPlan());

                manager.CodeGenerationPlans.Add(
                    target, 
                    codeGenerationPlan);
            }

            return true;
        }
    }
}
