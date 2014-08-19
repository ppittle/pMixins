//----------------------------------------------------------------------- 
// <copyright file="CalculateMixinAttributesForTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 29, 2014 8:07:45 PM</date> 
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

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Populates <see cref="TargetCodeBehindPlan.MixinAttributes"/>
    /// </summary>
    public class CalculateMixinAttributesForTargetCodeBehind : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            var attributesToIgnore = new[]
            {
                typeof (DoNotMixinAttribute),
                typeof(DebuggerTypeProxyAttribute),
                typeof(DefaultMemberAttribute)
            }.Select(t => t.ToIType(manager.CommonState.Context.TypeResolver.Compilation));

            
            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                cgp.TargetCodeBehindPlan.MixinAttributes =
                    cgp.MixinGenerationPlans.Values
                        .Select(mgp => mgp.MixinAttribute)
                        .SelectMany(att =>
                            att.Mixin
                                .GetAttributes()
                                .FilterOutNonInheritedAttributes()
                                .Where(a =>
                                    !attributesToIgnore.Contains(a.AttributeType) &&
                                    //Ignore unknown types (mostly microsoft internal attributes)
                                    !(a.AttributeType is UnknownType)));

            }

            return true;
        }
    }
}
