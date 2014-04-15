//----------------------------------------------------------------------- 
// <copyright file="GenerateCodeForStaticMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 2:29:14 PM</date> 
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
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.pMixinClassLevelGenerator.Steps
{
    /// <summary>
    /// Creates an interface that lists all the members the Target must implement in order
    /// to mixin the <see cref="pMixinGeneratorPipelineState.CurrentpMixinAttribute"/>.  This includes
    /// abstract methods and constructor initializers.
    /// </summary>
    public class GenerateMixinImplementationRequirementsInterface : IPipelineStep<pMixinGeneratorPipelineState>
    {
        

        public static string GetInitializationMethod(pMixinGeneratorPipelineState manager)
        {
            return "Initialize" + manager.CurrentpMixinAttribute.Mixin.FullName.Replace(".", "_");
        }

        public static string GetAbstractMemberImplementationName(IMember member)
        {
            return member.Name + "Implementation";
        }


        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            if (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsStatic)
                return true;

            //Create the TypeDeclaration
            var requirementsInterfaceDeclaration =
                new TypeDeclaration
                {
                    ClassType = ClassType.Interface,
                    Modifiers = Modifiers.Public,
                    Name =
                        "I"
                        + manager.CurrentpMixinAttribute.Mixin.GetNameAsIdentifier()
                        + "Requirements"
                };

            var interfaceNamespace = ExternalGeneratedNamespaceHelper.GenerateNamespace(manager);

            //Save the interface's FullName into the manager
            manager.CurrentMixinRequirementsInterface =
                interfaceNamespace
                .EnsureStartsWith("global::").EnsureEndsWith(".") +
                requirementsInterfaceDeclaration.Name;
        
            //Add the TypeDeclaration to the Generated Code Tree
            manager.BaseState.GeneratedCodeSyntaxTree.AddChildTypeDeclaration(
                requirementsInterfaceDeclaration, new NamespaceDeclaration(interfaceNamespace));

            //Create the Code Generator
            var requirementInterface = new CodeGeneratorProxy(
                requirementsInterfaceDeclaration, "");

            ProcessAbstractMembers(manager, requirementInterface);

            //Have the Target implement mixinRequirementsInterface
            manager.GeneratedClass.ImplementInterface(manager.CurrentMixinRequirementsInterface);

            return true;
        }

       

        private void ProcessAbstractMembers(
            pMixinGeneratorPipelineState manager, CodeGeneratorProxy requirementInterface)
        {
            foreach (var abstractMember in manager.CurrentMixinMembers
                .Select(x => x.Member)
                .Where(member => member.IsAbstract))
            {
                #region Process Methods
                if (abstractMember is IMethod)
                {
                    requirementInterface.CreateMethod(
                        string.Empty, //no modifier for interface member
                        abstractMember.ReturnType.GetOriginalFullNameWithGlobal(),
                        GetAbstractMemberImplementationName(abstractMember),
                        (abstractMember as IMethod).Parameters.ToKeyValuePair(),
                        string.Empty,
                        (abstractMember as IMethod).GetGenericMethodConstraints(
                            manager.BaseState.Context.TypeResolver.Compilation));
                }
                #endregion

                #region Process Properties
                else if (abstractMember is IProperty)
                {
                    requirementInterface.CreateProperty(
                        string.Empty, //no modifier for interface member
                        abstractMember.ReturnType.GetOriginalFullNameWithGlobal(),
                        GetAbstractMemberImplementationName(abstractMember),
                        (abstractMember as IProperty).CanGet ? "get;" : "",
                        (abstractMember as IProperty).CanSet ? "set;" : "");
                }
                #endregion
            }
        }

        [Obsolete("Moved to AddMixinConstructorRequirementDependency")]
        private void ProcessConstructors(pMixinGeneratorPipelineState manager, CodeGeneratorProxy requirementInterface)
        {
            bool hasANonParamaterlessConstructor =
                manager.CurrentpMixinAttribute.Mixin.GetConstructors()
                    .Any(c => !c.Parameters.Any());

            if (hasANonParamaterlessConstructor && 
                !manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract &&
                !manager.CurrentpMixinAttribute.ExplicitlyInitializeMixin)
                return;
            
            
            //Update ExplicitlyInitializeMixin - Changed mind, user has to always set 
            //ExplicitlyInitializeMixin to true even if mixin does not have parameterless constructor.
            //They could be using a custom MixinActivator that uses DI
            //manager.CurrentpMixinAttribute.ExplicitlyInitializeMixin = true;
            if (!manager.CurrentpMixinAttribute.ExplicitlyInitializeMixin)
                return;


            var requiredMixinWrapperType =
                (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract)
                    ? GenerateMixinSpecificAutoGeneratedClass.GetFullNameForChildType(
                        manager,
                        GenerateAbstractMixinMembersWrapperClass.GetWrapperClassName(manager))
                    : manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal();

            //generate the mixin initialization method
            requirementInterface.CreateMethod(
                string.Empty, //no modifier for interface member
                requiredMixinWrapperType,
                GetInitializationMethod(manager),
                new List<KeyValuePair<string, string>>(),
                "");
        }
    }
}
