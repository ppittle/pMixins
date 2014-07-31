//----------------------------------------------------------------------- 
// <copyright file="CodeGenerationPipelineState.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, January 20, 2014 11:13:19 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines
{
    public class CreateCodeGenerationPipelineState : IGenerateCodePipelineState, ICreateCodeGenerationPlanPipelineState, IResolveAttributesPipelineState, IParseSourceFilePipelineState, IPipelineCommonState
    {
        public CreateCodeGenerationPipelineState(ICodeGeneratorContext context)
            :this(context, new TypeInstanceActivator()){}

        public CreateCodeGenerationPipelineState(ICodeGeneratorContext context, ITypeInstanceActivator typeInstanceActivator)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(typeInstanceActivator, "typeInstanceActivator");

            Context = context;

            SourcePartialClassDefinitions = new List<TypeDeclaration>();
            AssemblyAttributes = new List<IAttribute>();
            SourcePartialClassAttributes = new Dictionary<TypeDeclaration, IList<IAttribute>>();
            CodeGenerationErrors = new List<CodeGenerationError>();

            TypeInstanceActivator = typeInstanceActivator;
            PartialClassLevelResolvedPMixinAttributes = new Dictionary<TypeDeclaration, IList<pMixinAttributeResolvedResultBase>>();

            CodeGenerationPlans = new Dictionary<TypeDeclaration, CodeGenerationPlan>();

            CodeBehindSyntaxTree = new SyntaxTree();
        }

        #region Pipeline Linking
        public IPipelineCommonState CommonState { get { return this; } }
        public IParseSourceFilePipelineState ParseSourceFilePipeline { get { return this; } }
        public IResolveAttributesPipelineState ResolveAttributesPipeline { get { return this; } }
        public ICreateCodeGenerationPlanPipelineState CreateCodeGenerationPlanPipeline { get { return this; } }
        #endregion

        #region IPipelineCommonState
        /// <summary>
        /// Returns information about the current code generation environment
        /// including the Visual Studio <see cref="Solution"/>
        /// and Target <see cref="CSharpFile"/>.
        /// </summary>
        public ICodeGeneratorContext Context { get; private set; }

        /// <summary>
        /// Collection of all Target <see cref="TypeDeclaration"/>s
        /// in the <see cref="ICodeGeneratorContext.Source"/>.
        /// </summary>
        public IList<TypeDeclaration> SourcePartialClassDefinitions { get; private set; }

        /// <summary>
        /// Collection of all <see cref="IAttribute"/>s
        /// found at the Assembly level.
        /// </summary>
        public IList<IAttribute> AssemblyAttributes { get; private set; }

        /// <summary>
        /// Mapping of <see cref="IAttribute"/>s that 
        /// to each <see cref="SourcePartialClassDefinitions"/>.
        /// </summary>
        public Dictionary<TypeDeclaration, IList<IAttribute>> SourcePartialClassAttributes { get; private set; }

        /// <summary>
        /// Provides a mechanism for 
        /// <see cref="IPipelineStep{TPipelineStateManager}"/>
        /// to add errors and warnings that should be bubbled up to 
        /// Visual Studio. 
        /// </summary>
        public IList<CodeGenerationError> CodeGenerationErrors { get; private set; }
        #endregion

        #region IResolveAttributesPipelineState

        /// <summary>
        /// A <see cref="ITypeInstanceActivator"/> that can 
        /// be used to create instances of <see cref="IAttribute"/>s.
        /// </summary>
        public ITypeInstanceActivator TypeInstanceActivator { get; private set; }

        /// <summary>
        /// Dictionary of <see cref="pMixinAttributeResolvedResultBase"/> representing
        /// the <see cref="IPMixinAttribute"/>s parsed in the source, and mapped to each
        /// parsed class.
        /// </summary>
        public Dictionary<TypeDeclaration, IList<pMixinAttributeResolvedResultBase>>
            PartialClassLevelResolvedPMixinAttributes { get; private set; }
        #endregion

        #region ICreateCodeGenerationPlanPipelineState
        /// <summary>
        ///  Dictionary of <see cref="CodeGenerationPlan"/> for each
        /// each <see cref="IPipelineCommonState.SourcePartialClassDefinitions"/>
        /// </summary>
        public Dictionary<TypeDeclaration, CodeGenerationPlan> CodeGenerationPlans { get; private set; }
        #endregion

        #region IGenerateCodePipelineState
        public SyntaxTree CodeBehindSyntaxTree { get; private set; }
        #endregion
    }
}
