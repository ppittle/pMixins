//----------------------------------------------------------------------- 
// <copyright file="AddMixinAttributesToTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 29, 2014 7:12:59 PM</date> 
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
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds all of the <see cref="TargetCodeBehindPlan.MixinAttributes"/> to 
    /// <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>:
    /// <code>
    /// <![CDATA[
    /// [IWasOnAMixin]
    /// public partial class Target{}
    /// ]]></code>
    /// </summary>
    public class AddMixinAttributesToTargetCodeBehind : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {

            new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration)
                .GeneratedClassSyntaxTree.Attributes
                .AddRange(

                    manager.CodeGenerationPlan.TargetCodeBehindPlan.MixinAttributes
                        .ConvertToAttributeAstTypes()
                        .Select(attributeAstType => new AttributeSection(attributeAstType)));

            return true;
        }
    }
}
