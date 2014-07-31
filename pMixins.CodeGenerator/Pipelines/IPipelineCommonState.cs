//----------------------------------------------------------------------- 
// <copyright file="IPipelineCommonState.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, November 10, 2013 12:26:14 AM</date> 
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
using CopaceticSoftware.Common.Patterns;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines
{
    /// <summary>
    /// The Pipeline State common to all Pipelines
    /// </summary>
    public interface IPipelineCommonState 
    {
        /// <summary>
        /// Returns information about the current code generation environment
        /// including the Visual Studio <see cref="Solution"/>
        /// and Target <see cref="CSharpFile"/>.
        /// </summary>
        ICodeGeneratorContext Context { get; }

        /// <summary>
        /// Collection of all Target <see cref="TypeDeclaration"/>s
        /// in the <see cref="ICodeGeneratorContext.Source"/>.
        /// </summary>
        IList<TypeDeclaration> SourcePartialClassDefinitions { get; }

        /// <summary>
        /// Collection of all <see cref="IAttribute"/>s
        /// found at the Assembly level.
        /// </summary>
        IList<IAttribute> AssemblyAttributes { get; }

        /// <summary>
        /// Mapping of <see cref="IAttribute"/>s that 
        /// to each <see cref="SourcePartialClassDefinitions"/>.
        /// </summary>
        Dictionary<TypeDeclaration, IList<IAttribute>> SourcePartialClassAttributes { get; }

        /// <summary>
        /// Provides a mechanism for 
        /// <see cref="IPipelineStep{TPipelineStateManager}"/>
        /// to add errors and warnings that should be bubbled up to 
        /// Visual Studio. 
        /// </summary>
        IList<CodeGenerationError> CodeGenerationErrors { get; }
    }
}
