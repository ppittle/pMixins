//----------------------------------------------------------------------- 
// <copyright file="AstTypeExtensions.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class AstTypeExtensions
    {
        public static string GetName(this AstType type)
        {
            var name = "<unknown>";

            if (type.TryCast<PrimitiveType>(pt => name = pt.Keyword))
                return name;

            if (type.TryCast<SimpleType>(st => name = st.Identifier))
                return name;

            if (type.TryCast<MemberType>(mt => name = mt.MemberName))
                return name;

            if (type.TryCast<ComposedType>(ct => name = ct.ToString()))
                return name;

            return name;
        }

        /// <summary>
        /// Takes an <see cref="AstType"/> node representing a
        /// Type and reworks it to include "global::" in front.
        /// </summary>
        /// <example>
        /// System.Object becomes global::System.Object
        /// </example>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AstType PromoteNameToGlobalNamespace(this AstType type)
        {
            if (null == type)
                return null;

            var rootNameSpaceElement =
               type.Descendants.First(x => !x.Descendants.OfType<SimpleType>().Any());

            var rootNameSpaceElementAsMemberType = 
                new MemberType(
                    new SimpleType("global"),
                    (rootNameSpaceElement.FirstChild as Identifier).Name)
                {
                    IsDoubleColon = true
                };

            rootNameSpaceElement.ReplaceWith(rootNameSpaceElementAsMemberType);

            return type;
        }
    }
}
