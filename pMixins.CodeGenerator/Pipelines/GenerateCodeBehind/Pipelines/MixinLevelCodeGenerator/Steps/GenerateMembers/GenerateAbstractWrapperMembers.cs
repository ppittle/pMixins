//----------------------------------------------------------------------- 
// <copyright file="GenerateAbstractWrapperMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 10:15:58 AM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Steps.GenerateMembers
{
    public class GenerateAbstractWrapperMembers : IPipelineStep<IGenerateCodePipelineState>
    {
        /// <summary>
        /// Variable name for the variable that references the target.  
        /// Is of Mixin specific IRequirements
        /// </summary>
        private const string RequirementsVariable = "_target";

        public bool PerformTask(IGenerateCodePipelineState manager)
        {
            var codeGenerator =
                new CodeGeneratorProxy(manager.AbstractMembersWrapper, "");

            CreateRequirementsDataMemberAndConstructor(codeGenerator, manager);
        }

        private void CreateRequirementsDataMemberAndConstructor(
            ICodeGeneratorProxy wrapperClass, IGenerateCodePipelineState manager)
        {
            wrapperClass.CreateDataMember(
                "private readonly",
                manager.CurrentMixinRequirementsInterface.EnsureStartsWith("global::"),
                RequirementsVariable);

            if (!manager.CurrentpMixinAttribute.ExplicitlyInitializeMixin &&
                manager.CurrentpMixinAttribute.Mixin.GetConstructors()
                .Any(c => !c.Parameters.Any()))
            {
                //No explicit initialization, so just add a simple constructor 
                //that takes a single parameter of type CurrentMixinRequirementsInterface

                wrapperClass.CreateConstructor(
                    "public",
                    new[]{new KeyValuePair<string, string>( 
                        manager.CurrentMixinRequirementsInterface.EnsureStartsWith("global::"),
                        RequirementsVariable.Replace("_",""))},
                    "",
                    RequirementsVariable + "= " + RequirementsVariable.Replace("_", "") + ";");

                return;
            }

            //Explicit initialization is needed, so wrap all existing constructors
            foreach (var constructor in manager.CurrentpMixinAttribute.Mixin.GetConstructors())
            {
                var updatedParameters =
                    constructor.Parameters.ToKeyValuePair();

                updatedParameters.Insert(0,
                    new KeyValuePair<string, string>(
                        manager.CurrentMixinRequirementsInterface.EnsureStartsWith("global::"),
                        RequirementsVariable.Replace("_", "")));

                wrapperClass.CreateConstructor(
                    "public",
                    updatedParameters,
                    ": base(" + string.Join(",", constructor.Parameters.Select(p => p.Name)) + ")",
                    RequirementsVariable + "= " + RequirementsVariable.Replace("_", "") + ";");
            }
        }

        private void ProcessMembers(ICodeGeneratorProxy wrapperClass, IGenerateCodePipelineState manager)
        {
            var proxyMemberHelper = new CodeGeneratorProxyMemberHelper(wrapperClass,
                manager.BaseState.Context.TypeResolver.Compilation);

            proxyMemberHelper.CreateMembers(
                manager.CurrentMixinMembers.GetUnimplementedAbstractMembers(),
                generateMemberModifier: member => member.IsPublic ? "public override" : "protected override",
                generateReturnTypeFunc: member => member.ReturnType.GetOriginalFullNameWithGlobal(),
                baseObjectIdentifierFunc: member => RequirementsVariable,
                baseObjectMemberNameFunc: member => GenerateMixinImplementationRequirementsInterface.GetAbstractMemberImplementationName(member));


            //Create public wrappers for the protected methods
            proxyMemberHelper.CreateMembers(
                manager.CurrentMixinMembers.GetUnimplementedAbstractMembers().Where(m => m.IsProtected),
                generateMemberModifier: member => "public",
                generateReturnTypeFunc: member => member.ReturnType.GetOriginalFullNameWithGlobal(),
                generateMemberNameFunc: member => GetProtectedMemberWrapperMemberName(member),
                baseObjectIdentifierFunc: member => "this");


        }
    }
}
