//----------------------------------------------------------------------- 
// <copyright file="CalculateTargetSpecificMixinInterfacesToImplement.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 29, 2014 11:29:54 AM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Populates <see cref="TargetCodeBehindPlan.MixinInterfaces"/>
    /// </summary>
    public class CalculateTargetSpecificMixinInterfacesToImplement : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            var doNotMixinIType =
                typeof (DoNotMixinAttribute).ToIType(
                    manager.CommonState.Context.TypeResolver.Compilation);

            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                cgp.TargetCodeBehindPlan.MixinInterfaces =
                    cgp.MixinGenerationPlans.Values
                        .SelectMany(mgp => mgp.MixinAttribute.Mixin.GetAllBaseTypes())
                        .Where(bt => bt.Kind == TypeKind.Interface)
                        //Filter out interfaces decorated with DoNotMixin
                        .Where(bt => !bt.IsDecoratedWithAttribute(doNotMixinIType))
                        .Distinct();
            }

            return true;
        }
    }
}
