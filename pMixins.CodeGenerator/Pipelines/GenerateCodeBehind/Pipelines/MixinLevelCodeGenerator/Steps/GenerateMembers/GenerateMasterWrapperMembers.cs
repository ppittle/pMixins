//----------------------------------------------------------------------- 
// <copyright file="GenerateMasterWrapperMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 6:03:42 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using Mono.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    public class GenerateMasterWrapperMembers : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var proxyMemberHelper =
                new MasterWrapperCodeGeneratorProxyMemberHelper(
                    new CodeGeneratorProxy(manager.MasterWrapper, ""),
                    manager.CommonState.Context.TypeResolver.Compilation);


            ProcessStaticMembers(proxyMemberHelper, manager);

            ProcessProtectedAbstractMembers(proxyMemberHelper, manager);

            ProcessVirtualMembers(proxyMemberHelper, manager);

            ProcessRegularMembers(proxyMemberHelper, manager);

            return true;
        }

        private void ProcessStaticMembers(
            MasterWrapperCodeGeneratorProxyMemberHelper proxyMemberHelper, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            proxyMemberHelper.CreateMembers(
                  manager.MixinGenerationPlan.MasterWrapperPlan.StaticMembers,
                   generateMemberModifier: 
                        member => "internal static",
                   baseObjectIdentifierFunc:
                       member =>
                           manager.MixinGenerationPlan.AbstractWrapperPlan.GenrateAbstractWrapper
                           ? manager.AbstractMembersWrapper.Name
                           : manager.MixinGenerationPlan.MixinAttribute.Mixin.GetOriginalFullNameWithGlobal()
               );
        }

        private void ProcessProtectedAbstractMembers(
            MasterWrapperCodeGeneratorProxyMemberHelper proxyMemberHelper, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            proxyMemberHelper.CreateMembers(
                  manager.MixinGenerationPlan.MasterWrapperPlan.ProtectedAbstractMembers,
                  generateMemberModifier: 
                        member => "internal",
                  baseObjectIdentifierFunc:
                        member => manager.MixinGenerationPlan.MasterWrapperPlan.MixinInstanceDataMemberName,
                  baseObjectMemberNameFunc: 
                        member => member.ImplementationDetails.ProtectedAbstractMemberPromotedToPublicMemberName
            );
        }

        private void ProcessVirtualMembers(
            MasterWrapperCodeGeneratorProxyMemberHelper proxyMemberHelper, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            proxyMemberHelper.CreateMembers(
                manager.MixinGenerationPlan.MasterWrapperPlan.VirtualMembers,
                generateMemberModifier:
                        member => "internal",
                baseObjectIdentifierFunc:
                        member => "this",
                baseObjectMemberNameFunc:
                        member => member.ImplementationDetails.VirtualMemberFunctionName
                 );  
        }

        private void ProcessRegularMembers(
            MasterWrapperCodeGeneratorProxyMemberHelper proxyMemberHelper, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            proxyMemberHelper.CreateMembers(
                   manager.MixinGenerationPlan.MasterWrapperPlan.RegularMembers,
                   generateMemberModifier: 
                        member => "internal",
                   baseObjectIdentifierFunc:
                        member => manager.MixinGenerationPlan.MasterWrapperPlan.MixinInstanceDataMemberName);
        }
    }
}
