//----------------------------------------------------------------------- 
// <copyright file="CalculateTargetSpecificImplicitConversionTypes.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 28, 2014 8:17:16 PM</date> 
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
using ICSharpCode.NRefactory.TypeSystem;
using JetBrains.Annotations;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Populates <see cref="TargetCodeBehindPlan.ImplicitCoversionPlans"/>
    /// </summary>
    public class CalculateTargetSpecificImplicitConversionTypes : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                cgp.TargetCodeBehindPlan.ImplicitCoversionPlans =
                    cgp.MixinGenerationPlans.Values
                        .Select(CollectImplicitConversionOperator)
                        .Where(x => null != x)
                        .Distinct();
            }

            return true;
        }

        [CanBeNull]
        private ImplicitConversionPlan CollectImplicitConversionOperator(MixinGenerationPlan mgp)
        {
            if (mgp.MixinAttribute.Mixin.GetDefinition().IsPublic)
                return new ImplicitConversionPlan
                {
                    MixinGenerationPlan = mgp,
                    ConversionTargetType = mgp.MixinAttribute.Mixin,
                };

            //fall back to the first base type in Mixin that is public
            //otherwise return null
            return mgp.MixinAttribute.Mixin.GetDefinition()
                .GetAllBaseTypes()
                .Where(
                    t =>
                        t.GetDefinition().IsPublic &&
                        t.GetDefinition().FullName.ToLower() != "system.object")
                .Select(
                    t => 
                            new ImplicitConversionPlan
                        {
                            MixinGenerationPlan = mgp,
                            ConversionTargetType = t
                        })
                .FirstOrDefault();
        }
    }
}
