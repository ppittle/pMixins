//----------------------------------------------------------------------- 
// <copyright file="AddIMixinConstructorRequirementInterfaceToTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 30, 2014 11:28:56 AM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using CopaceticSoftware.pMixins.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds an <see cref="IMixinConstructorRequirement{TMixin}"/> interface for every
    /// <see cref="MixinGenerationPlan"/> (where <see cref="MixinGenerationPlan.AddIMixinConstructorRequirementInterface"/>
    /// is true) to 
    /// <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>:
    /// <code>
    /// <![CDATA[
    /// public partial class Target :  CopaceticSoftware.pMixins.Infrastructure.IMixinConstructorRequirement<ExampleMixin>
    /// {     
    /// }
    /// ]]></code>
    /// </summary>
    public class AddIMixinConstructorRequirementInterfaceToTargetCodeBehind :
        IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var codeGeneratorProxy =
                new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration);

            var mixinConstructorRequirements =
                typeof (IMixinConstructorRequirement<>).ToIType(manager.CommonState.Context.TypeResolver.Compilation)
                    .GetDefinition();

            manager.CodeGenerationPlan.MixinGenerationPlans.Values
                .Where(mgp => mgp.AddIMixinConstructorRequirementInterface)
                .Map(mgp =>

                    codeGeneratorProxy.ImplementInterface(
                        new ParameterizedType(
                            mixinConstructorRequirements, 
                            new[] {mgp.MixinAttribute.Mixin})
                        .GetOriginalFullNameWithGlobal(
                            manager.CommonState.Context.TypeResolver.Resolve(
                                manager.TargetSourceTypeDeclaration)
                            .Type))
                );

            return true;
        }
    }
}
