//----------------------------------------------------------------------- 
// <copyright file="CreateProtectedWrapperPlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 4:52:15 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Creates a <see cref="ProtectedWrapperPlan"/> for every
    /// <see cref="MixinGenerationPlan"/>.
    /// </summary>
    public class CreateProtectedWrapperPlan : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var mixinPlan in
                manager.CodeGenerationPlans.SelectMany(
                    x => x.Value.MixinGenerationPlans))
            {
                mixinPlan.Value.ProtectedWrapperPlan = BuildPlan(mixinPlan.Value);
            }

            return true;
        }

        private ProtectedWrapperPlan BuildPlan(MixinGenerationPlan mixinPlan)
        {
            return new ProtectedWrapperPlan
            {
                GenrateProtectedWrapper = !mixinPlan.MixinAttribute.Mixin.IsStaticOrSealed(),

                Constructors = mixinPlan.MixinAttribute.Mixin
                                .GetConstructors()
                                .Where(c => c.IsProtected),

                Members = mixinPlan.Members.Where(m =>
                    m.Member.IsProtected && !m.Member.IsAbstract),

                GenerateProtectedWrapperInExternalNamespace = !mixinPlan.MixinAttribute.Mixin.GetDefinition().IsPrivate,

                ProtectedWrapperClassName = mixinPlan.MixinAttribute.Mixin.GetNameAsIdentifier() + "ProtectedMembersWrapper"
            };
        }
    }
}
