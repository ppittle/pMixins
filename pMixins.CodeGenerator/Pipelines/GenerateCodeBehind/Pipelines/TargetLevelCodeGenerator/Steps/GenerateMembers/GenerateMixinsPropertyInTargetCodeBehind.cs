//----------------------------------------------------------------------- 
// <copyright file="GenerateMixinsPropertyInTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, July 27, 2014 3:59:56 PM</date> 
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

using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Generates:
    /// <code>
    /// <![CDATA[
    /// public partial class Target{
    /// 
    ///     private __Mixins _____mixins;
    ///	    private __Mixins ___mixins {
    ///		    get {
    ///			    if (null == _____mixins) {
    ///				    lock (__Mixins.____Lock) {
    ///					    if (null == _____mixins) {
    ///						    _____mixins = new __Mixins (this);
    /// 						_____mixins.__ActivateMixinDependencies (this);
    ///				    	}
    ///				    }
    ///			    }
    ///			    return _____mixins;
    ///		    }
    ///	    } 
    /// }
    /// ]]></code>
    /// </summary>
    public class GenerateMixinsPropertyInTargetCodeBehind : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var codeGenerator =
                new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration);

            var backingFiledName = "__" + manager.CodeGenerationPlan.MixinsPropertyName;

            //create backing field
            codeGenerator.CreateDataMember(
                modifiers:
                    "private",
                dataMemberTypeFullName:
                    manager.CodeGenerationPlan.MixinsClassName,
                dataMemberName:
                    backingFiledName
                );

            //create private property
            codeGenerator.CreateProperty(
                modifier:
                    "private",
                returnTypeFullName:
                    manager.CodeGenerationPlan.MixinsClassName,
                propertyName:
                    manager.CodeGenerationPlan.MixinsPropertyName,
                getterMethodBody:
                    string.Format(
                        @"
                                get 
                                {{
                                    if (null == {0})
                                    {{
                                        lock({1}.{2})
                                        {{
                                                if (null == {0})
                                                {{
                                                    {0} = new {1}(this);
                                                    {0}.{3}(this);
                                                }}
                                        }}
                                    }}
    
                                    return {0};
                                }}
                            ",
                        backingFiledName,
                        manager.CodeGenerationPlan.MixinsClassName,
                        manager.CodeGenerationPlan.MixinsLockVariableName,
                        manager.CodeGenerationPlan.MixinsActivateMixinDependenciesMethodName)
                ,
                setterMethodBody:
                    string.Empty //no setter
                );

            return true;
        }
    }
}
