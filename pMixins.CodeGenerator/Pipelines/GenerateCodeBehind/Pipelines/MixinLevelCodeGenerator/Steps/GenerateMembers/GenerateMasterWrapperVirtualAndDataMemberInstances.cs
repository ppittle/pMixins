//----------------------------------------------------------------------- 
// <copyright file="GenerateMasterWrapperConstructorAndDataMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, July 27, 2014 8:23:04 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds the virtual member and mixin instance data members
    /// to the <see cref="MixinLevelCodeGeneratorPipelineState.MasterWrapper"/>.
    /// <code>
    /// <![CDATA[
    /// public class MixinMasterWrapper  
    /// {
    ///     public readonly global::Test.Mixin _mixinInstance;
    /// 
    ///		public Func<int, int> MyVirtualMethodFunc {get;set;}
    /// }
    /// ]]></code>
    /// </summary>
    public class GenerateMasterWrapperVirtualAndDataMemberInstances : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var codeGenerator = new CodeGeneratorProxy(manager.MasterWrapper);

            GenerateMixinInstanceDataMember(codeGenerator, manager);

            GenerateVirtualFuncDataMembers(codeGenerator, manager);

            return true;
        }

        private void GenerateMixinInstanceDataMember(CodeGeneratorProxy codeGenerator, MixinLevelCodeGeneratorPipelineState manager)
        {
            codeGenerator.CreateDataMember(
                modifiers:
                //must be public for implicit conversion operator
                    "public",
                dataMemberTypeFullName:
                    manager.MixinGenerationPlan.MasterWrapperPlan.MixinInstanceTypeFullName,
                dataMemberName:
                    MasterWrapperPlan.MixinInstanceDataMemberName);
        }

        private void GenerateVirtualFuncDataMembers(CodeGeneratorProxy codeGenerator, MixinLevelCodeGeneratorPipelineState manager)
        {
            foreach (var mw in manager.MixinGenerationPlan.MasterWrapperPlan.VirtualMembers)
            {
                if (mw.Member is IMethod)
                {
                    var virtualFuncReturnType =
                        (mw.Member as IMethod).ReturnType.Kind == TypeKind.Void
                            ? typeof(Action).GetOriginalFullNameWithGlobal()
                            : string.Format("global::System.Func<{0}>",
                                string.Join(",",
                                    (mw.Member as IMethod).Parameters
                                        .Select(x => x.Type.GetOriginalFullNameWithGlobal())
                                    .Concat(new[] { mw.Member.ReturnType.GetOriginalFullNameWithGlobal() })));

                    codeGenerator.CreateProperty(
                        modifier:
                            "public",
                        returnTypeFullName:
                            virtualFuncReturnType,
                        propertyName:
                            mw.ImplementationDetails.VirtualMemberFunctionName,
                        getterMethodBody:
                            "get;",
                        setterMethodBody:
                            "set;");
                }
                else if (mw.Member is IProperty)
                {
                    //Get
                    if ((mw.Member as IProperty).CanGet && !(mw.Member as IProperty).Getter.IsPrivate)
                        codeGenerator.CreateProperty(
                            modifier:
                                "public",
                            returnTypeFullName:
                                "global::System.Func<" + mw.Member.ReturnType.GetOriginalFullNameWithGlobal() + ">",
                            propertyName:
                                mw.ImplementationDetails.VirtualMemberFunctionName + "Get",
                            getterMethodBody:
                                "get;",
                            setterMethodBody:
                                "set;");
                    //Set
                    if ((mw.Member as IProperty).CanSet && !(mw.Member as IProperty).Setter.IsPrivate)
                        codeGenerator.CreateProperty(
                            modifier:
                                "public",
                            returnTypeFullName:
                                "global::System.Action<" + mw.Member.ReturnType.GetOriginalFullNameWithGlobal() + ">",
                            propertyName:
                                mw.ImplementationDetails.VirtualMemberFunctionName + "Set",
                            getterMethodBody:
                                "get;",
                            setterMethodBody:
                                "set;");
                }
            }
        }
    }
}
