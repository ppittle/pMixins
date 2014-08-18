//----------------------------------------------------------------------- 
// <copyright file="SetMemberImplementationDetails.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 4:43:33 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// May need to separate this out.
    /// </summary>
    public class SetMemberImplementationDetails : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                cgp.Members
                    .Map(m => SetDetails(cgp, m.ImplementationDetails));
            }

            return true;
        }

        private void SetDetails(
            CodeGenerationPlan cgp,
            MemberImplementationDetails implDetails)
        {
            var member = implDetails.ParentMemberWrapper.Member;

            if (member.IsExplicitInterfaceImplementation)
                implDetails.ExplicitInterfaceImplementationType =
                    member.ImplementedInterfaceMembers.First().DeclaringType;
                    
            implDetails.ImplementInTargetAsAbstract =
                member.IsAbstract &&
                implDetails.ParentMemberWrapper.MixinAttribute.KeepAbstractMembersAbstract;
            
            var sourceClassAlsoDefinesMember =
                cgp.SourceClassMembers.Contains(member, new MemberExtensions.MemberEqualityComparer());

            implDetails.RequirementsInterfaceImplementationName =
                (   member.IsAbstract && 
                    !sourceClassAlsoDefinesMember && 
                    !implDetails.ImplementInTargetAsAbstract)
                ? member.Name + "Implementation"
                : member.Name;

            if (member.IsAbstract && 
                member.IsProtected )
            {
                implDetails.ProtectedAbstractMemberPromotedToPublicMemberName =
                    member.Name + "_Public";

            }
        }
    }
}
