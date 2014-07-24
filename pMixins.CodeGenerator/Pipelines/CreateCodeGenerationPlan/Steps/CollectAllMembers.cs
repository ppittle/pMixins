//----------------------------------------------------------------------- 
// <copyright file="CollectAllMembers.cs" company="Copacetic Software"> 
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
using System.Linq;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    public class CollectAllMembers : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var target in manager.CommonState.SourcePartialClassDefinitions)
            {
                var memberWrappers =
                    manager.ResolveAttributesPipeline.PartialClassLevelResolvedPMixinAttributes[target]
                        .OfType<pMixinAttributeResolvedResult>()
                        .SelectMany(CollectMemberWrappers)
                        .ToList();

                manager.CodeGenerationPlans[target].Members = 
                    memberWrappers;   
            }

            return true;
        }

        private IEnumerable<MemberWrapper> CollectMemberWrappers(pMixinAttributeResolvedResult mixinAttribute)
        {
            return 
                //Collect all applicable types
                mixinAttribute.Mixin.GetAllBaseTypes()
                    .Union(new[] {mixinAttribute.Mixin})
                    //Collect all members
                    .SelectMany(t => CollectMemberWrappers(t, mixinAttribute));
        }

        private IEnumerable<MemberWrapper> CollectMemberWrappers(IType parentType, 
            pMixinAttributeResolvedResult mixinAttribute)
        {
            return
                parentType.GetMembers()
                    .Select(m =>
                        new MemberWrapper
                        {
                            DeclaringType = parentType,
                            Member = m,
                            MixinAttribute = mixinAttribute
                        });

        }
    }
}
