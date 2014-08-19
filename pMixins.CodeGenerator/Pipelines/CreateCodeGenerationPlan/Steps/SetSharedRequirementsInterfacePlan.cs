//----------------------------------------------------------------------- 
// <copyright file="SetSharedRequirementsInterfacePlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 25, 2014 6:10:58 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Extensions;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    public class SetSharedRequirementsInterfacePlan : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var codeGenerationPlan in manager.CodeGenerationPlans.Values)
            {
                codeGenerationPlan.SharedRequirementsInterfacePlan = new RequirementsInterfacePlan
                {
                    RequirementsInterfaceName = "ISharedRequirements"
                };

                var abstractMembers =
                    codeGenerationPlan.MixinGenerationPlans.Values
                        .Where(mgp => mgp.MixinAttribute.EnableSharedRequirementsInterface)
                        .SelectMany(mgp => mgp.Members)
                        .Where(mw => 
                            mw.Member.IsAbstract &&
                            !mw.ImplementationDetails.ImplementExplicitly)
                        .FilterMemberWrappers(codeGenerationPlan.SourceClassMembers)
                        .ToList();

                var groupedByCount =
                    abstractMembers
                        .DistinctMemberWrappers()
                        .Select(mw =>
                            new KeyValuePair<MemberWrapper, int>(
                                mw,
                                abstractMembers.Count(x =>
                                    new MemberWrapperExtensions.MemberWrapperEqualityComparer().Equals(mw, x))));

                codeGenerationPlan.SharedRequirementsInterfacePlan.Members =
                    groupedByCount
                        .Where(x => x.Value > 1)
                        .Select(x => x.Key)
                        .ToList();
            }

            return true;
        }
    }
}
