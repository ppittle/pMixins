//----------------------------------------------------------------------- 
// <copyright file="GenerateRequiredInterfaceMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 25, 2014 6:10:58 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds members to the <see cref="MixinLevelCodeGeneratorPipelineState.RequirementsInterface"/>
    /// based on the <see cref="MixinGenerationPlan.RequirementsInterfacePlan"/>.
    /// <code>
    /// <![CDATA[
    /// public interface IMixinRequirements
    /// {
    ///    void AbstractMethodImplementation();
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateRequiredInterfaceMembers : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var proxyMemberHelper =
                new InterfaceCodeGeneratorProxyMemberHelper(
                    new CodeGeneratorProxy(manager.RequirementsInterface),
                    manager.CommonState.Context.TypeResolver.Compilation);

            proxyMemberHelper.CreateMembers(
                    manager.MixinGenerationPlan.RequirementsInterfacePlan
                    .Members);

            return true;
        }
    }
}
