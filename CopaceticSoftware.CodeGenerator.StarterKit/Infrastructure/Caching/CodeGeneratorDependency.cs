//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 13, 2014 5:09:17 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    public class CodeGeneratorDependency
    {
        public CSharpFile TargetFile { get; set; }

        public List<CSharpFile> FileDependencies { get; set; }

        public List<IType> TypeDependencies { get; set; }

        public CodeGeneratorDependency()
        {
            FileDependencies = new List<CSharpFile>();
            TypeDependencies = new List<IType>();
        }
    }
}
