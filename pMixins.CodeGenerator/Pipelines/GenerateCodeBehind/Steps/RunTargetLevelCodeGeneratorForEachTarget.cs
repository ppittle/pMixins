//----------------------------------------------------------------------- 
// <copyright file="GenerateCodeForEachTargetInSourceFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 11:34:24 AM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Steps
{
    /// <summary>
    /// Iterate through each <see cref="IPipelineCommonState.SourcePartialClassDefinitions"/>
    /// and launch the <see cref="TargetLevelCodeGenerator"/>.
    /// </summary>
    public class RunTargetLevelCodeGeneratorForEachTarget : IPipelineStep<IGenerateCodePipelineState>
    {
        public bool PerformTask(IGenerateCodePipelineState manager)
        {
            foreach (var target in manager.CommonState.SourcePartialClassDefinitions)
            {
                var targetLevelCodeGeneratorPipeline =
                    new TargetLevelCodeGeneratorPipeline(manager)
                    {
                        TargetSourceTypeDeclaration = target
                    };

                new TargetLevelCodeGenerator().PerformTask(targetLevelCodeGeneratorPipeline);
            }

            return true;
        }
    }
}
