//----------------------------------------------------------------------- 
// <copyright file="ICodeGeneratorContext.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 5:48:10 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;

namespace CopaceticSoftware.CodeGenerator.StarterKit
{
    public interface ICodeGeneratorContext
    {
        CSharpFile Source { get; }
        CSharpAstResolver TypeResolver { get; }
    }

    public class CodeGeneratorContext : ICodeGeneratorContext
    {
        public CSharpFile Source { get; set; }

        public CSharpAstResolver TypeResolver { get; set; }
    }
}
