//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorResponse.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, November 9, 2013 6:07:44 PM</date> 
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
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure
{
    public class CodeGeneratorResponse 
    {
        public CodeGeneratorResponse()
        {
            Errors = new List<CodeGenerationError>();
            LogMessages = new List<string>();
            GeneratedCodeSyntaxTree = new SyntaxTree();
        }

        public SyntaxTree GeneratedCodeSyntaxTree { get; set; }

        public TimeSpan CodeGeneratorExecutionTime { get; set; }
        public IList<CodeGenerationError> Errors { get; set; }
        public IEnumerable<string> LogMessages { get; set; }

        public ICodeGeneratorContext CodeGeneratorContext { get; set; }
    }
}
