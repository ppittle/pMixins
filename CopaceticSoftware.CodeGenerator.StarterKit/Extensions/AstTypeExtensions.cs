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
