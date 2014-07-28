//----------------------------------------------------------------------- 
// <copyright file="GenerateImplicitConversionOperatorMethods.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 28, 2014 10:46:19 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Create the Static Implicit Operator 
    /// for <see cref="TargetCodeBehindPlan.ImplicitCoversionPlans"/>
    /// inside <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>:
    /// <code>
    /// <![CDATA[
    /// public partial class Target
    /// { 
    ///     public static implicit operator global::Test.ExampleMixin(Target target)
    ///     {
    ///         return target.__mixins.Test_ExampleMixin._mixinInstance;
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateImplicitConversionOperatorMethods: IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var codeGenerator =
                new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration);

            const string targetInstanceMethodParameterName = "target";

            foreach (var conversionPlan in manager.CodeGenerationPlan.TargetCodeBehindPlan.ImplicitCoversionPlans)
            {
                codeGenerator.CreateMethod(
                    modifier:
                        "public static implicit",
                    returnTypeFullName:
                        "operator",
                    methodName:
                        conversionPlan.ConversionTargetType.GetOriginalFullNameWithGlobal(),
                    parameters:
                        new []
                        {
                            new KeyValuePair<string, string>(
                                manager.TargetCodeBehindTypeDeclaration.Name, 
                                targetInstanceMethodParameterName)
                        },
                    methodBody:
                        string.Format(
                            "return {0}.{1}.{2}.{3};",
                            targetInstanceMethodParameterName,
                            manager.CodeGenerationPlan.TargetCodeBehindPlan.MixinsPropertyName,
                            conversionPlan.MixinGenerationPlan.MasterWrapperPlan.MasterWrapperInstanceNameInMixinsContainer,
                            MasterWrapperPlan.MixinInstanceDataMemberName)
                );
            }


            return true;


        }
    }
}
