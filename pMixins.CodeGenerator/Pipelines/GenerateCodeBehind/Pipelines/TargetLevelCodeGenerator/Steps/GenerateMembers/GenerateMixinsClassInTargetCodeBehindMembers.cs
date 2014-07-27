//----------------------------------------------------------------------- 
// <copyright file="GenerateMixinsClassInTargetCodeBehindMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, July 27, 2014 5:41:36 PM</date> 
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
using System.Text;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Extensions;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using CopaceticSoftware.pMixins.Infrastructure;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Generates the members for <see cref="CodeGenerationPlan.MixinsClassName"/> inside
    /// <see cref="TargetLevelCodeGeneratorPipelineState.MixinsClassInTargetCodeBehind"/>:
    /// <code>
    /// <![CDATA[
    /// private sealed class __Mixins
    /// {
    ///     public static global::System.Object ____Lock = new global::System.Object ();
    ///		
    ///     public readonly ExampleMixinMasterWrapper Test_ExampleMixin;
    ///		
    ///     public __Mixins (Target target)
    ///		{
    ///			Test_ExampleMixin = MixinActivatorFactory.GetCurrentActivator()
    ///             .CreateInstance<ExampleMixinMasterWrapper>( (IExampleMixinRequirements) target );
    ///		}
    ///		
    /// 
    ///		public void __ActivateMixinDependencies (Target target)
    ///		{
    ///			Test_ExampleMixin.__ActivateMixinDependencies (target);
    ///		}
    /// }
    /// ]]></code>
    /// </summary>
    public class GenerateMixinsClassInTargetCodeBehindMembers : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var mixinsClassCodeGenerator =
                new CodeGeneratorProxy(manager.MixinsClassInTargetCodeBehind);

            AddLockDataMember(mixinsClassCodeGenerator, manager.CodeGenerationPlan);

            AddMixinDataMembers(mixinsClassCodeGenerator, manager);

            AddConstructor(mixinsClassCodeGenerator, manager);

            AddActivateMixinDependenciesMethod(mixinsClassCodeGenerator, manager);

            return true;
        }

        private void AddLockDataMember(CodeGeneratorProxy mixinsClassCodeGenerator, CodeGenerationPlan codeGenerationPlan)
        {
            mixinsClassCodeGenerator.CreateDataMember(
                modifiers:
                    "public static",
                dataMemberTypeFullName:
                    "global::System.Object",
                dataMemberName:
                    codeGenerationPlan.MixinsLockVariableName,
                initializerExpression:
                    "= new global::System.Object();");
        }

        private void AddMixinDataMembers(CodeGeneratorProxy mixinsClassCodeGenerator, TargetLevelCodeGeneratorPipelineState manager)
        {
            foreach (var mixin in manager.CodeGenerationPlan.MixinGenerationPlans.Values)
            {
                mixinsClassCodeGenerator.CreateDataMember(
                    modifiers:
                        "public",
                    dataMemberTypeFullName:
                        GetMasterWrapperFullTypeName(mixin),
                    dataMemberName:
                        mixin.MasterWrapperPlan.MasterWrapperInstanceNameInMixinsContainer);
            }
        }

        private void AddConstructor(CodeGeneratorProxy mixinsClassCodeGenerator, TargetLevelCodeGeneratorPipelineState manager)
        {
            const string targetInstanceConstructorParameterName = "target";

            var constructorBodyStatements = new StringBuilder();

            foreach (var mixin in manager.CodeGenerationPlan.MixinGenerationPlans.Values)
            {
                constructorBodyStatements.AppendFormat(
                    "{0} = {1};",

                    mixin.MasterWrapperPlan.MasterWrapperInstanceNameInMixinsContainer,

                    TypeExtensions.GenerateActivationExpression(
                        typeFullName:
                            GetMasterWrapperFullTypeName(mixin),
                        constructorArgs:
                            string.Format(
                                "({0}.{1}) {2}",
                                mixin.ExternalMixinSpecificAutoGeneratedNamespaceName,
                                mixin.RequirementsInterfacePlan.RequirementsInterfaceName,
                                targetInstanceConstructorParameterName))
                    );
            }

            mixinsClassCodeGenerator.CreateConstructor(
                modifiers:
                    "public",
                parameters:
                    new[]
                    {
                        new KeyValuePair<string, string>(
                            manager.TargetSourceTypeDeclaration.Name, 
                            targetInstanceConstructorParameterName) 
                    },
                constructorInitializer:
                    string.Empty,
                constructorBody:
                    constructorBodyStatements.ToString());
        }

        private void AddActivateMixinDependenciesMethod(CodeGeneratorProxy mixinsClassCodeGenerator, TargetLevelCodeGeneratorPipelineState manager)
        {
            const string targetInstanceMethodParameterName = "target";

            var methodBodyStatements = new StringBuilder();

            foreach (var mixin in manager.CodeGenerationPlan.MixinGenerationPlans.Values)
            {
                methodBodyStatements.AppendFormat(
                    "{0}.{1}({2};",
                    mixin.MasterWrapperPlan.MasterWrapperInstanceNameInMixinsContainer,
                    manager.CodeGenerationPlan.MixinsActivateMixinDependenciesMethodName,
                    targetInstanceMethodParameterName);
            }


            mixinsClassCodeGenerator.CreateMethod(
                modifier:
                    "public",
                returnTypeFullName:
                    "void",
                methodName:
                    manager.CodeGenerationPlan.MixinsActivateMixinDependenciesMethodName,
                parameters:
                    new[]
                    {
                        new KeyValuePair<string, string>(
                            manager.TargetSourceTypeDeclaration.Name,
                            targetInstanceMethodParameterName)
                    },
                methodBody:
                    methodBodyStatements.ToString());
        }

        private string GetMasterWrapperFullTypeName(MixinGenerationPlan mixinPlan)
        {
            return
                mixinPlan.CodeGenerationPlan.GlobalAutoGeneratedContainerClassName.EnsureEndsWith(".") +
                mixinPlan.MixinLevelAutoGeneratedContainerClass.EnsureEndsWith(".") +
                mixinPlan.MasterWrapperPlan.MasterWrapperClassName;
        }
    }
}
