//----------------------------------------------------------------------- 
// <copyright file="CreateMixinsClassInTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, July 27, 2014 4:27:36 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.CreateTypeDeclarations
{
    /// <summary>
    /// Creates the <see cref="CodeGenerationPlan.MixinsClassName"/> inside
    /// <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>
    /// and assigns it to <see cref="TargetLevelCodeGeneratorPipelineState.MixinsClassInTargetCodeBehind"/>
    /// <code>
    /// <![CDATA[
    /// public partial class Target{
    ///    private sealed class __Mixins { }
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class CreateMixinsClassInTargetCodeBehind : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var mixinsClassDeclaration =
                new TypeDeclaration
                {
                    ClassType = ClassType.Class,
                    Modifiers = Modifiers.Private | Modifiers.Sealed,
                    Name = manager.CodeGenerationPlan.MixinsClassName,
                };

            new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration)
                .AddNestedType(mixinsClassDeclaration);

            manager.MixinsClassInTargetCodeBehind = mixinsClassDeclaration;

            return true;
        }
    }
}
