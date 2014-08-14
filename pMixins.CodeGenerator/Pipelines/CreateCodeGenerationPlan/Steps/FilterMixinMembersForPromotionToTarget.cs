//----------------------------------------------------------------------- 
// <copyright file="FilterMixinMembersForPromotionToTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, July 26, 2014 1:45:28 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Extensions;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Controls which <see cref="MixinGenerationPlan.Members"/> will be assigned to 
    /// <see cref="MixinGenerationPlan.MembersPromotedToTarget"/> for eventual inclusion in the 
    /// Target's code-behind.
    /// </summary>
    public class FilterMixinMembersForPromotionToTarget : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                var allMixinGenPlans =
                    cgp.MixinGenerationPlans.Values.ToList();

                for (int i = 0; i < allMixinGenPlans.Count; i++)
                {
                    if (i == 0)
                    {
                        allMixinGenPlans[i].MembersPromotedToTarget =
                            allMixinGenPlans[i].Members.ToList();

                        continue;
                    }

                    var previouslyPromotedMembers =
                        allMixinGenPlans.Take(i).SelectMany(x => x.MembersPromotedToTarget);

                    allMixinGenPlans[i].MembersPromotedToTarget =
                        allMixinGenPlans[i].Members
                            .Where(m => previouslyPromotedMembers.All(
                                ppm => !ppm.Member.EqualsMember(m.Member)))
                            .ToList();
                }

                //ensure there wont be any collisions with members
                //already defined in Target
                allMixinGenPlans.Map(
                    agp => agp.MembersPromotedToTarget =
                        agp.MembersPromotedToTarget
                            .FilterMembersFromType(cgp.SourceClass, manager.CommonState));
            }

            return true;
        }
    }
}
