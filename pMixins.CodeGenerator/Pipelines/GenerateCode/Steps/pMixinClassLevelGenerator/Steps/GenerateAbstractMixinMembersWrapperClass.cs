﻿//----------------------------------------------------------------------- 
// <copyright file="GenerateAbstractMixinMembersWrapperClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 2, 2014 11:40:40 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.pMixinClassLevelGenerator.Steps
{
    public class GenerateAbstractMixinMembersWrapperClass : IPipelineStep<pMixinGeneratorPipelineState>
    {
        private const string RequirementsVariable = "_target";

        public static string GetProtectedMemberWrapperMemberName(IMember member)
        {
            return (member.IsProtected && member.IsAbstract)
                ? member.Name + "_Public"
                : member.Name;
        }

        public static string GetWrapperClassName(pMixinGeneratorPipelineState manager)
        {
            return manager.CurrentpMixinAttribute.Mixin.GetNameAsIdentifier() + "AbstractWrapper";
        }

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            if (manager.CurrentpMixinAttribute.Mixin.IsStaticOrSealed())
                return true;

            var wrapperClassDeclaration = new TypeDeclaration
                                          {
                                              ClassType = ClassType.Class,
                                              Modifiers = GenerateProtectedMixinMembersWrapperClass.GetMixinTypeModifiers(manager),
                                              Name = GetWrapperClassName(manager)
                                          };
            

            var simpleType = new SimpleType(
                (Identifier)
                manager.CurrentMixinProtectedMembersWrapperClass.Descendants.OfType<Identifier>().First().Clone());
            
            wrapperClassDeclaration.BaseTypes.Add(simpleType);

            manager.CurrentMixinAbstractMembersWrapperClass = wrapperClassDeclaration;

            ICodeGeneratorProxy wrapperClass;

            if (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract)
            {
                //Protected/Abstract Wrappers will all be created in the external
                //namespace (Mixin is not allowed to be private)
                manager.BaseState.GeneratedCodeSyntaxTree.AddChildTypeDeclaration(
                    wrapperClassDeclaration,
                    new NamespaceDeclaration(ExternalGeneratedNamespaceHelper.GenerateNamespace(manager)));

                wrapperClass = new CodeGeneratorProxy(wrapperClassDeclaration, "");
            }
            else
            {
                //Protected/Abstract Wrappers will all be created as 
                //a nested type inside Target (in case Mixin is private)

                wrapperClass = manager.CurrentAutoGeneratedTypeDeclaration.AddNestedType(wrapperClassDeclaration);
            }

            CreateRequirementsDataMemberAndConstructor(wrapperClass, manager);

            ProcessMembers(wrapperClass, manager);

            return true;
        }

        private void CreateRequirementsDataMemberAndConstructor(
            ICodeGeneratorProxy wrapperClass, pMixinGeneratorPipelineState manager)
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
                    new []{new KeyValuePair<string, string>( 
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
                        RequirementsVariable.Replace("_","")));

                wrapperClass.CreateConstructor(
                    "public",
                    updatedParameters,
                    ": base(" + string.Join(",",constructor.Parameters.Select(p => p.Name)) + ")",
                    RequirementsVariable + "= " + RequirementsVariable.Replace("_", "") + ";");
            }
        }

        private void ProcessMembers(ICodeGeneratorProxy wrapperClass, pMixinGeneratorPipelineState manager)
        {
            var proxyMemberHelper = new CodeGeneratorProxyMemberHelper(wrapperClass,
                manager.BaseState.Context.TypeResolver.Compilation);

            proxyMemberHelper.CreateMembers(
                manager.CurrentMixinMembers.Select( x=> x.Member).Where(m => m.IsAbstract && !m.IsStatic),
                generateMemberModifier: member => member.IsPublic ? "public override" : "protected override",
                generateReturnTypeFunc: member => member.ReturnType.GetOriginalFullNameWithGlobal(),
                baseObjectIdentifierFunc: member => RequirementsVariable,
                baseObjectMemberNameFunc: member => GenerateMixinImplementationRequirementsInterface.GetAbstractMemberImplementationName(member));


            //Create public wrappers for the protected methods
            proxyMemberHelper.CreateMembers(
                manager.CurrentMixinMembers.Select(x => x.Member).Where(m => m.IsAbstract && !m.IsStatic && m.IsProtected),
                generateMemberModifier: member => "public",
                generateReturnTypeFunc: member => member.ReturnType.GetOriginalFullNameWithGlobal(),
                generateMemberNameFunc: member => GetProtectedMemberWrapperMemberName(member),
                baseObjectIdentifierFunc: member => "this");

           
        }
    }
}
