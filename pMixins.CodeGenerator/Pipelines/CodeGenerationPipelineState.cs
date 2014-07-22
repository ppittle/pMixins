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
using CopaceticSoftware.Common.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines
{
    public class CodeGenerationPipelineState : ICodeGenerationPipelineState
    {
        public CodeGenerationPipelineState(ICodeGeneratorContext context)
            :this(context, new TypeInstanceActivator()){}

        public CodeGenerationPipelineState(ICodeGeneratorContext context, ITypeInstanceActivator typeInstanceActivator)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(typeInstanceActivator, "typeInstanceActivator");

            Context = context;

            SourcePartialClassDefinitions = new List<TypeDeclaration>();
            AssemblyAttributes = new List<IAttribute>();
            SourcePartialClassAttributes = new Dictionary<TypeDeclaration, IList<IAttribute>>();
            CodeGenerationErrors = new List<CodeGenerationError>();

            TypeInstanceActivator = typeInstanceActivator;
            PartialClassLevelResolvedpMixinAttributes = new Dictionary<TypeDeclaration, IList<pMixinAttributeResolvedResultBase>>();

            GeneratedClasses = new List<ICodeGeneratorProxy>();
            GeneratedCodeSyntaxTree = new SyntaxTree();
        }
        
        public ICodeGeneratorContext Context { get; private set; }

        public IList<TypeDeclaration> SourcePartialClassDefinitions { get; private set; }
        public IList<IAttribute> AssemblyAttributes { get; private set; }
        public Dictionary<TypeDeclaration, IList<IAttribute>> SourcePartialClassAttributes { get; private set; }
        public IList<CodeGenerationError> CodeGenerationErrors { get; private set; }
        
        public ITypeInstanceActivator TypeInstanceActivator { get; private set; }

        /// <summary>
        /// Dictionary of <see cref="pMixinAttributeResolvedResultBase"/> representing
        /// the <see cref="IpMixinAttribute"/>s parsed in the source, and mapped to each
        /// parsed class.
        /// </summary>
        public Dictionary<TypeDeclaration, IList<pMixinAttributeResolvedResultBase>> PartialClassLevelResolvedpMixinAttributes { get; private set; }

        public IList<ICodeGeneratorProxy> GeneratedClasses { get; private set; }
        public SyntaxTree GeneratedCodeSyntaxTree { get; private set; }
    }
}
