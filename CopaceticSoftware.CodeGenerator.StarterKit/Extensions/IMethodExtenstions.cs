//----------------------------------------------------------------------- 
// <copyright file="IMethodExtenstions.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class IMethodExtenstions
    {
        public static string GetGenericMethodConstraints(this IMethod method, ICompilation compilation)
        {
            var sb = new StringBuilder();

            foreach (var typeParam in method.Parts.SelectMany(p =>
                p.TypeParameters.OfType<DefaultUnresolvedTypeParameter>()))
            {
                var resolvedParts = new List<string>();

                if (typeParam.HasDefaultConstructorConstraint)
                    resolvedParts.Add("new()");

                if (typeParam.HasReferenceTypeConstraint)
                    resolvedParts.Add("class");

                if (typeParam.HasValueTypeConstraint)
                    resolvedParts.Add("struct");

                resolvedParts.AddRange(
                    typeParam.Constraints.Resolve(compilation.TypeResolveContext)
                        .Select(x => x.GetOriginalFullName()));

                if (resolvedParts.Any())
                    sb.Append("where ").Append(typeParam.Name).Append(" : ").Append(string.Join(",", resolvedParts));
            }

            return sb.ToString();
        }

        public static string GetReturnString(this IMethod method)
        {
            return (null == method || method.ReturnType.Kind == TypeKind.Void)
                ? ""
                : "return";
        }
    }
}


