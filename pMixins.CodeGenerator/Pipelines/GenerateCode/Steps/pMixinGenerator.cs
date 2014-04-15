﻿//----------------------------------------------------------------------- 
// <copyright file="pMixinGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 4:50:44 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.pMixinClassLevelGenerator.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps
{
    public class pMixinGenerator : IPipelineStep<ICodeGenerationPipelineState>
    {
        private readonly IPipelineStep<pMixinGeneratorPipelineState>[] _universalPreamblePipeline =
            {
                new ResolveAllMixinMembers(),
                new GenerateMixinsContainerClass(),
                new GenerateAutoGeneratedContainerClass() 
            };

        private readonly IPipelineStep<pMixinGeneratorPipelineState>[] _pMixinAttributePipeline =
            {
                new GenerateMixinSpecificAutoGeneratedClass(),
                new GenerateMixinImplementationRequirementsInterface(), 
                new GenerateProtectedMixinMembersWrapperClass(), 
                new GenerateAbstractMixinMembersWrapperClass(), 
                new GenerateMixinMasterWrapperClass(), 
                new GenerateMixinDataMembersInMixinsContainerClassMixin(),
                new GenerateMembers(),
                new GenerateImplicitConversionOperator(),
                new AddMixinConstructorRequirementDependency(),
                new AddIContainMixinInterface(), 
                new AddMixinClassAttributesToGeneratedClass(), 
                new AddMixinInterfacesToInterfaceList()
            };

        private readonly IPipelineStep<pMixinGeneratorPipelineState>[] _universalPostamblePipeline =
            {
                new GenerateMixinsContainerClassConstructor(),
                new AddInterfacesToGeneratedContainerClass() 
            };
        

        public bool PerformTask(ICodeGenerationPipelineState manager)
        {
            foreach (var sourceClass in manager.SourcePartialClassDefinitions)
            {
                //Generate the new partial class definition, create 
                //a code generator for later steps to use

                var newClassDeclaration =
                    new TypeDeclaration
                        {
                            ClassType = ClassType.Class,
                            Modifiers = sourceClass.Modifiers, // this should include partial
                            Name = sourceClass.Name
                        };

                manager.GeneratedCodeSyntaxTree.AddChildTypeDeclaration
                    (newClassDeclaration, sourceClass.GetParent<NamespaceDeclaration>());

                var generatedClass = new CodeGeneratorProxy(
                    newClassDeclaration,
                    manager.Context.Source.OriginalText);

                var mixinGeneratorState =
                    new pMixinGeneratorPipelineState
                        {
                            SourceClass = sourceClass,
                            GeneratedClass = generatedClass,
                            BaseState = manager
                        };

                if (!_universalPreamblePipeline.RunPipeline(mixinGeneratorState,
                           haltOnStepFailing: step => true))
                    return false;

                foreach (var pMixinAttribute in manager.PartialClassLevelResolvedpMixinAttributes[sourceClass]
                    .OfType<pMixinAttributeResolvedResult>())
                {
                    mixinGeneratorState.CurrentpMixinAttribute = pMixinAttribute;
                    mixinGeneratorState.CurrentMixinMembers =
                        mixinGeneratorState.MixinMembers[pMixinAttribute.Mixin];
                    mixinGeneratorState.CurrentMixinAbstractMembersWrapperClass = null;
                    mixinGeneratorState.CurrentMixinProtectedMembersWrapperClass = null;
                    mixinGeneratorState.CurrentMixinMasterWrapperFullTypeName = string.Empty;
                    mixinGeneratorState.CurrentMixinInstanceVariableAccessor = string.Empty;
                    mixinGeneratorState.CurrentMixinRequirementsInterface = string.Empty;
                    

                    if (!_pMixinAttributePipeline.RunPipeline(mixinGeneratorState,
                           haltOnStepFailing: step => true))
                        return false;    
                }

                if (!_universalPostamblePipeline.RunPipeline(mixinGeneratorState,
                         haltOnStepFailing: step => true))
                    return false;
            }

            return true;
        }
    }
}
