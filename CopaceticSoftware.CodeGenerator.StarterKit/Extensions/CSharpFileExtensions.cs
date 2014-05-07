//----------------------------------------------------------------------- 
// <copyright file="CSharpFileExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 6:44:07 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class CSharpFileExtensions
    {
        public static IEnumerable<IType> ResolveTypes(this CSharpFile file)
        {
            var resolver = file.CreateResolver();

            return file.SyntaxTree.GetClassDefinitions()
                .Select(x => resolver.Resolve(x))
                .Where(x => !x.IsError)
                .Select(x => x.Type)
                .ToArray();
        }
    }
}
