//----------------------------------------------------------------------- 
// <copyright file="CSharpFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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
using System.Diagnostics;
using CopaceticSoftware.Common.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    [DebuggerDisplay("{FileName}")]
    public class CSharpFile
    {
        public readonly string FileName;
        public readonly string OriginalText;
        public readonly SyntaxTree SyntaxTree;
        public readonly CSharpUnresolvedFile UnresolvedTypeSystemForFile;
        public readonly CSharpProject Project;
        public readonly IEnumerable<Error> Errors;

        public CSharpFile(CSharpProject project, string fileName, string sourceCode)
        {
            Ensure.ArgumentNotNull(project, "project");

            Project = project;
            FileName = fileName;
            OriginalText = sourceCode;

            var parser = new CSharpParser(Project.CompilerSettings);
            
            SyntaxTree = parser.Parse(sourceCode, fileName);

            Errors = parser.HasErrors
                ? parser.ErrorsAndWarnings
                : new List<Error>(0);

            UnresolvedTypeSystemForFile = SyntaxTree.ToTypeSystem();
        }

        public CSharpAstResolver CreateResolver()
        {
            return new CSharpAstResolver(Project.Compilation, SyntaxTree, UnresolvedTypeSystemForFile);
        }
    }
}
