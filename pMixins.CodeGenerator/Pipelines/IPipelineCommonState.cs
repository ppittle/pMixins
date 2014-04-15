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
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines
{
    /// <summary>
    /// The Pipeline State common to all Pipelines
    /// </summary>
    public interface IPipelineCommonState 
    {
        ICodeGeneratorContext Context { get; }

        IList<TypeDeclaration> SourcePartialClassDefinitions { get; }

        IList<IAttribute> AssemblyAttributes { get; }
        Dictionary<TypeDeclaration, IList<IAttribute>> SourcePartialClassAttributes { get; }

        IList<CodeGenerationError> CodeGenerationErrors { get; }
    }
}
