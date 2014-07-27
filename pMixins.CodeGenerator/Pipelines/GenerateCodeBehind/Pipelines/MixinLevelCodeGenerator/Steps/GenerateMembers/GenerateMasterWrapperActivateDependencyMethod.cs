//----------------------------------------------------------------------- 
// <copyright file="GenerateMasterWrapperActivateDependencyMethod.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, July 27, 2014 8:47:15 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds the <see cref="CodeGenerationPlan.MixinsActivateMixinDependenciesMethodName"/> method 
    /// to the <see cref="MixinLevelCodeGeneratorPipelineState.MasterWrapper"/>.
    /// <code>
    /// <![CDATA[
    /// public class MixinMasterWrapper  
    /// {
    ///     public void __ActivateMixinDependencies (Target target)
    ///		{
    ///			((IMixinDependency<Test.Dependency>)_mixinInstance).Dependency = target;
    ///			((IMixinDependency<Test.Dependency>)_mixinInstance).OnDependencySet ();
    ///		}
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateMasterWrapperActivateDependencyMethod : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var codeGenerator = new CodeGeneratorProxy(manager.MasterWrapper);

            CreateActivateDependencyMethod(codeGenerator, manager.MixinGenerationPlan);

            return true;
        }

        private void CreateActivateDependencyMethod(CodeGeneratorProxy codeGenerator, MixinGenerationPlan mixinGenerationPlan)
        {
            const string targetInstanceMethodParameterName = "target";

            var methodBodyStatements = new StringBuilder();

            foreach (var dependency in mixinGenerationPlan.MasterWrapperPlan.MixinDependencies)
            {
                methodBodyStatements.AppendFormat(
                    @"  (({0}){1}).Dependency = {2};
                        (({0}){1}).OnDependencySet();",

                    dependency.GetOriginalFullNameWithGlobal(),
                    MasterWrapperPlan.MixinInstanceDataMemberName,
                    targetInstanceMethodParameterName);
            }

            codeGenerator.CreateMethod(
                modifier:
                    "public",
                returnTypeFullName:
                    "void",
                methodName:
                    mixinGenerationPlan.CodeGenerationPlan.MixinsActivateMixinDependenciesMethodName,
                parameters:
                    new[]
                    {
                        new KeyValuePair<string, string>(
                            mixinGenerationPlan.CodeGenerationPlan.SourceClass.GetFullTypeName(), 
                            targetInstanceMethodParameterName)
                    },
                methodBody:
                    methodBodyStatements.ToString()
                        //make sure method has at least an empty body
                        .IfEmpty("{}"));
        }
    }
}
