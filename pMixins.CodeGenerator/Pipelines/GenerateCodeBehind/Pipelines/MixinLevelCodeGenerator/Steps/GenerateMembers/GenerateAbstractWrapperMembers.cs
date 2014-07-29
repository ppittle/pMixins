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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds members to the Abstract Wrapper:
    /// <code>
    /// <![CDATA[
    /// //Class created in earlier step.
    /// public class MixinAbstractWrapper : MixinProtectedWrapper
    /// {
    ///     IMixinRequirements _target;
    /// 
    ///     public MixinAbstractWrapper(IMixinRequirements target)
    ///     {
    ///          _target = target;
    ///     }
    /// 
    ///     public override string PublicAbstractMethod()
    ///     {
    ///          return _target.PublicAbstractMethodImplementation();
    ///     }
    /// 
    ///     //Special case for protected abstract members
    ///     //because they can't be made public in the Protected Wrapper
    ///     protected override string ProtectedAbstractMethod()
    ///     {
    ///          return _target.ProtectedAbstractMethodImplementation();
    ///     }
    /// 
    ///     public string ProtectedAbstractMethod_Public()
    ///     {
    ///          return ProtectedAbstractMethod();
    ///     } 
    /// } 
    /// ]]></code>
    /// </summary>
    public class GenerateAbstractWrapperMembers : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        /// <summary>
        /// Variable name for the variable that references the target.  
        /// Is of Mixin specific IRequirements
        /// </summary>
        private const string RequirementsVariable = "_target";

        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            if (!manager.MixinGenerationPlan.AbstractWrapperPlan.GenrateAbstractWrapper)
                return true;

            var codeGenerator =
                new CodeGeneratorProxy(manager.AbstractMembersWrapper);

            CreateRequirementsDataMember(codeGenerator, manager);

            CreateRequirementsDataMemberAndConstructor(codeGenerator, manager);

            ProcessMembers(codeGenerator, manager);

            return true;
        }

        private void CreateRequirementsDataMember(
            ICodeGeneratorProxy codeGenerator, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            codeGenerator.CreateDataMember(
               modifiers:
                           "private readonly",
               dataMemberTypeFullName:
                           manager.RequirementsInterface
                               .GetFullTypeName().EnsureStartsWith("global::"),
               dataMemberName:
                           RequirementsVariable);
        }

        private void CreateRequirementsDataMemberAndConstructor(
            ICodeGeneratorProxy abstractWrapperCodeGenerator, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            var requirementsVariableConstructorParameterName =
                    RequirementsVariable.Replace("_", "");

            var requirementsVariableTypeFullName =
                manager.RequirementsInterface
                    .GetFullTypeName().EnsureStartsWith("global::");

            var requirementsVariableConstructorParamater =
                new KeyValuePair<string, string>(
                    //param type
                    requirementsVariableTypeFullName,
                    //param var name
                    requirementsVariableConstructorParameterName);

            //_target = target;
            var requirementsVariableAssignmentExpression =
                string.Format("{0} = {1};",
                    //data member 
                    RequirementsVariable,
                    //equals constructor param name
                    requirementsVariableConstructorParameterName);
            
            if (manager.MixinGenerationPlan.AbstractWrapperPlan.WrapAllConstructors)
            {
                var allConstructors =
                    manager.MixinGenerationPlan.MixinAttribute.Mixin.GetConstructors();

                foreach (var constructor in allConstructors)
                {
                    //Add requirementsVariableConstructorParameterName
                    //as first constructor argument
                    var updatedParameters =
                        new []
                        {
                            requirementsVariableConstructorParamater
                        }
                        .Union(
                            constructor.Parameters.ToKeyValuePair())
                        .ToList();

                    abstractWrapperCodeGenerator.CreateConstructor(
                        modifiers:
                            "public",
                        parameters:
                            updatedParameters,
                        constructorInitializer:
                            //pass original constructor arguments to original Mixin constructor
                            ": base(" + string.Join(",", constructor.Parameters.Select(p => p.Name)) + ")",
                        constructorBody:
                            requirementsVariableAssignmentExpression
                    );
                }
            }
            else
            {
                //Add a simple constructor 
                //that takes a single parameter of type CurrentMixinRequirementsInterface
                //public AbstractWrapper(IMixinRequirements target)
                //{
                //    _target = target;
                //}
                abstractWrapperCodeGenerator
                    .CreateConstructor(
                        modifiers:
                            "public",
                        parameters:
                            new[]
                            {
                                new KeyValuePair<string, string>(
                                    //param type
                                    requirementsVariableTypeFullName,
                                    //param var name
                                    requirementsVariableConstructorParameterName)
                            },
                        constructorInitializer:
                            string.Empty,
                        constructorBody:
                            requirementsVariableAssignmentExpression
                        );
            }
        }

        private void ProcessMembers(
            ICodeGeneratorProxy abstractWrapperCodeGenerator, 
            MixinLevelCodeGeneratorPipelineState manager)
        {
            var proxyMemberHelper = 
                new CodeGeneratorProxyMemberHelper(
                    abstractWrapperCodeGenerator,
                    manager.CommonState.Context.TypeResolver.Compilation);

            proxyMemberHelper.CreateMembers(
                manager.MixinGenerationPlan.AbstractWrapperPlan.Members,
                generateMemberModifier: member => member.Member.GetModifiersString(overrideModifiers: "override"),
                generateReturnTypeFunc: member => member.Member.ReturnType.GetOriginalFullNameWithGlobal(),
                baseObjectIdentifierFunc: member => RequirementsVariable,
                baseObjectMemberNameFunc: member => member.ImplementationDetails.RequirementsInterfaceImplementationName);


            //There is a special case for protected abstract method.
            //Abstract methods can not have their access modifier 
            //changed directly, so it is necessary to create
            //a public wrapper method.

            var abstractMembersToCreateAPublicWrapper =
                manager.MixinGenerationPlan.AbstractWrapperPlan.Members
                    .Where(m =>
                        !string.IsNullOrEmpty(
                            m.ImplementationDetails.ProtectedAbstractMemberPromotedToPublicMemberName));

            proxyMemberHelper.CreateMembers(
                abstractMembersToCreateAPublicWrapper,
                generateMemberModifier: m => "public",
                generateReturnTypeFunc: m => m.Member.ReturnType.GetOriginalFullNameWithGlobal(),
                generateMemberNameFunc: m => m.ImplementationDetails.ProtectedAbstractMemberPromotedToPublicMemberName,
                //proxy to the protected member created above
                baseObjectIdentifierFunc: m => "this");


        }
    }
}
