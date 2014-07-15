//----------------------------------------------------------------------- 
// <copyright file="GenerateMixinsContainerActivateMixinDependenciesMethod.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 15, 2014 2:24:54 PM</date> 
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

using System;
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PostClassGeneration
{
    /// <summary>
    /// Generates the Activate Mixin Dependency for the Mixins Container Class.
    /// 
    /// The class itself is created in <see cref="GenerateMixinsContainerClass"/>
    /// <code>
    /// <![CDATA[
    /// public __Mixins (Target host)
    ///		{
    ///			public void __ActivateMixinDependencies (Target host)
	///			{
	///				Test_Mixin.__ActivateMixinDependencies (host);
    ///			}
    ///		}
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateMixinsContainerActivateMixinDependenciesMethod : IPipelineStep<pMixinGeneratorPipelineState>
    {
        private const string mixinMethodParameterName = "host";

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var mixins =
                manager.BaseState.PartialClassLevelResolvedpMixinAttributes[
                    manager.SourceClass]
                    .OfType<pMixinAttributeResolvedResult>();

            manager.MixinContainerClassGeneratorProxy
                .CreateMethod(
                    modifier: "public",
                    returnTypeFullName: "void",
                    methodName: GenerateMixinMasterWrapperClass.ActivateMixinDependenciesMethodName,
                    parameters: new[]
                    {
                        new KeyValuePair<string, string>(
                            manager.GeneratedClass.ClassName,
                            mixinMethodParameterName),
                    },
                    methodBody:

                        string.Join(Environment.NewLine,
                            mixins
                                .Select(x =>
                                    string.Format("{0}.{1}({2});",
                                        x.Mixin.GetFullNameAsIdentifier(),
                                        GenerateMixinMasterWrapperClass.ActivateMixinDependenciesMethodName,
                                        mixinMethodParameterName
                                        )))
                );

            return true;
        }
    }
}
