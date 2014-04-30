//----------------------------------------------------------------------- 
// <copyright file="CSharpFile.cs" company="Copacetic Software"> 
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
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD.NRefactory
{
    /// <remarks>
    /// Copied from the NRefactory StringIndexOf sample
    /// http://www.codeproject.com/Articles/408663/Using-NRefactory-for-analyzing-Csharp-code
    /// </remarks>
    public class CSharpFile
    {
        public readonly string FileName;
        public readonly string OriginalText;

        private readonly IEnumerable<Error> _errors;
        public bool HasErrors
        {
            get { return _errors.Any(); }
        }

        public readonly SyntaxTree SyntaxTree;
        public readonly CSharpUnresolvedFile UnresolvedTypeSystemForFile;
        public CSharpProject _project { get; private set; }

        public CSharpFile(CSharpProject project, string fileName)
            : this(project, fileName, File.ReadAllText(fileName)) { }

        public CSharpFile(CSharpProject project, string fileName, string sourceCode)
        {
            _project = project;
            FileName = fileName;

            var parser = new CSharpParser(project.CompilerSettings);

            // Keep the original text around; we might use it for a refactoring later
            OriginalText = sourceCode;
            SyntaxTree = parser.Parse(OriginalText, fileName);

            _errors = parser.HasErrors
                ? parser.ErrorsAndWarnings
                : new List<Error>(0);

            UnresolvedTypeSystemForFile = SyntaxTree.ToTypeSystem();
        }

        public CSharpAstResolver CreateResolver()
        {
            return new CSharpAstResolver(_project.Compilation, SyntaxTree, UnresolvedTypeSystemForFile);
        }
    }
}
