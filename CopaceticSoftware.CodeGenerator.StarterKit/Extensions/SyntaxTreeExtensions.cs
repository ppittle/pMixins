//----------------------------------------------------------------------- 
// <copyright file="SyntaxTreeExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, January 13, 2014 4:39:58 PM</date> 
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
using System.Linq;
using CopaceticSoftware.Common.Infrastructure;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class SyntaxTreeExtensions
    {
        public static IEnumerable<TypeDeclaration> GetClassDefinitions(this AstNode tree)
        {
            Ensure.ArgumentNotNull(tree, "tree");

            return tree.Descendants.OfType<TypeDeclaration>();
        }

        public static IEnumerable<TypeDeclaration> GetPartialClasses(this AstNode tree)
        {
            Ensure.ArgumentNotNull(tree, "tree");

            return
                tree.GetClassDefinitions()
                .Where(td =>
                       td.ModifierTokens.Any(
                           mod => mod.GetText().Equals(
                               "partial", StringComparison.InvariantCultureIgnoreCase)));
        }

        public static void AddChildTypeDeclaration(this AstNode tree, TypeDeclaration newClass,
            NamespaceDeclaration parentNamespace = null)
        {
            if (null != parentNamespace)
            {
                var newNamespaceNode = new NamespaceDeclaration(
                    parentNamespace.Name);

                newNamespaceNode.AddMember(newClass);

                tree.AddChild(newNamespaceNode, SyntaxTree.MemberRole);
            }
            else
            {
                tree.AddChild(newClass, Roles.TypeMemberRole);
            }
        }
    }
}
