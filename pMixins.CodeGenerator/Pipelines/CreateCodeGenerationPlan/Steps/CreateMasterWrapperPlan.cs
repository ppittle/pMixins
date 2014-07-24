//----------------------------------------------------------------------- 
// <copyright file="CreateMasterWrapperPlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 6:03:42 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Creates a <see cref="MasterWrapperPlan"/> for every
    /// <see cref="MixinGenerationPlan"/>.
    /// </summary>
    public class CreateMasterWrapperPlan : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var mixinPlan in
                manager.CodeGenerationPlans.SelectMany(
                    x => x.Value.MixinGenerationPlans))
            {
                mixinPlan.Value.MasterWrapperPlan = BuildPlan(mixinPlan.Value);
            }

            return true;
        }

        private MasterWrapperPlan BuildPlan(MixinGenerationPlan mixinPlan)
        {
            return new MasterWrapperPlan
            {
                ProtectedAbstractMembers =
                    mixinPlan.Members
                        .Where(m =>
                            (m.Member.IsAbstract || m.Member.IsOverride)
                            && m.Member.IsProtected),

                RegularMembers =
                    mixinPlan.Members
                        .Where(m =>
                                !m.Member.IsStatic &&
                                !(m.Member.IsAbstract && m.Member.IsProtected) &&
                                !m.Member.IsOverride &&
                                !m.Member.IsOverridable &&
                                !m.Member.IsVirtual),

                StaticMembers = 
                    mixinPlan.Members
                        .Where(m => m.Member.IsStatic),
                        
                VirtualMembers = 
                     mixinPlan.Members
                        .Where(m  => 
                             m.Member.IsVirtual ||
                            (
                                m.Member.IsOverride ||
                                m.Member.IsOverridable &&
                                ! m.Member.IsProtected
                            ))
            };
        }
    }
}
