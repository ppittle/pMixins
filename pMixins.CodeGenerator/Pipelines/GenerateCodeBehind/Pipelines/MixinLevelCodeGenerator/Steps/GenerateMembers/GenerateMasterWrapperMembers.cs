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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds members to the <see cref="MixinLevelCodeGeneratorPipelineState.MasterWrapper"/>.
    /// </summary>
    public class GenerateMasterWrapperMembers : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var proxyMemberHelper =
                new MasterWrapperCodeGeneratorProxyMemberHelper(
                    new CodeGeneratorProxy(manager.MasterWrapper),
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
                           ? manager.AbstractMembersWrapper.GetFullTypeName()
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
                        member => MasterWrapperPlan.MixinInstanceDataMemberName,
                  baseObjectMemberNameFunc: 
                        member => member.ImplementationDetails.ProtectedAbstractMemberPromotedToPublicMemberName
            );
        }

        private void ProcessVirtualMembers(
            MasterWrapperCodeGeneratorProxyMemberHelper proxyMemberHelper, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            var virtualMethods =
                manager.MixinGenerationPlan.MasterWrapperPlan.VirtualMembers
                    .Where(mw => mw.Member is IMethod);

            var virtualPropertiesAndFields =
                manager.MixinGenerationPlan.MasterWrapperPlan.VirtualMembers
                    .Where(mw => mw.Member is IProperty || mw.Member is IField);

            #region Methods
                proxyMemberHelper.CreateMembers(
                    virtualMethods,
                    generateMemberModifier:
                            member => "internal",
                    baseObjectIdentifierFunc:
                            member => "this",
                    baseObjectMemberNameFunc:
                            member => member.ImplementationDetails.VirtualMemberFunctionName
                     );
            #endregion

            #region Properties

            //virtual properties need to be created directly.

            foreach (var virtualProp in virtualPropertiesAndFields)
            {
                proxyMemberHelper.CodeGeneratorProxy.CreateProperty(
                    modifier:
                        "internal",
                    returnTypeFullName:
                        virtualProp.Member.ReturnType.GetOriginalFullNameWithGlobal(),
                    propertyName:
                        virtualProp.Member.Name,
                    getterMethodBody:
                        ( ! (virtualProp.Member is IProperty) || 
                          ! (virtualProp.Member as IProperty).CanGet)
                          ? string.Empty
                          : string.Format(
                                "get{{ return base.ExecutePropertyGet(\"{0}\", () => {1}Get();}}",
                                virtualProp.Member.Name,
                                virtualProp.ImplementationDetails.VirtualMemberFunctionName),
                    setterMethodBody:
                         (!(virtualProp.Member is IProperty) ||
                          !(virtualProp.Member as IProperty).CanSet)
                          ? string.Empty
                          : string.Format(
                                "set{{ base.ExecutePropertySet(\"{0}\", value, (v) => {1}Set(v);}}",
                                virtualProp.Member.Name,
                                virtualProp.ImplementationDetails.VirtualMemberFunctionName)
                    );
            }

            #endregion
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
                        member => MasterWrapperPlan.MixinInstanceDataMemberName);
        }
    }
}
