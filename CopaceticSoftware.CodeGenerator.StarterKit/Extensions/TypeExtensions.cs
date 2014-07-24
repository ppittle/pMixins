//----------------------------------------------------------------------- 
// <copyright file="TypeExtensions.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class TypeExtensions
    {
        public static AstType ToAstSyntaxType(this IType type)
        {
            Ensure.ArgumentNotNull(type, "type");

            return new TypeSystemAstBuilder().ConvertType(type);
        }

        /// <summary>
        /// "global::" + <see cref="GetOriginalFullName(ICSharpCode.NRefactory.TypeSystem.IType)"/>
        /// </summary>
        /// <param name="type">
        /// The type to work on.
        /// </param>
        /// <param name="rootDefinition">
        /// Optional parameter that is used to look up the <see cref="DefaultTypeParameter"/>
        /// </param>
        public static string GetOriginalFullNameWithGlobal(this IType type, IType rootDefinition = null)
        {
            if (type is VoidTypeDefinition)
                return "void";

            if (type is ParameterizedType)
                return
                    "global::" +
                    type.FullName +
                       "<" +
                       string.Join(
                            ",", 
                            (type as ParameterizedType).TypeArguments
                                .Select(x => x.GetOriginalFullNameWithGlobal(rootDefinition)))
                       + ">";


            var defaultTypeParam = type as DefaultTypeParameter;
            if (null != defaultTypeParam)
            {
                var rootDefinitionParamType = rootDefinition as ParameterizedType;

                if (null == rootDefinitionParamType || null == defaultTypeParam.Owner)
                    return type.Name;

                //Special handling for resolving the type of generic type parameters
                return rootDefinitionParamType.TypeArguments[defaultTypeParam.Index]
                    .GetOriginalFullNameWithGlobal(rootDefinition);
            }

            //Fall back case
            return "global::" + type.GetOriginalFullName();
        }

        /// <summary>
        /// Prints a generic type the way it would look in source code.
        /// Includes the namespace.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/6402864/c-pretty-type-name-function
        /// </remarks>
        public static string GetOriginalFullName(this IType type)
        {
            Ensure.ArgumentNotNull(type, "type");

            if (type is VoidTypeDefinition)
                return "void";

            if (type is ArrayType)
            {
                var arrayType = type as ArrayType;

                return arrayType.ElementType.GetOriginalFullName() + arrayType.NameSuffix;
            }

            var astType = type.ToAstSyntaxType();

            if (astType is PrimitiveType)
                return "System." + (astType as PrimitiveType).KnownTypeCode;

            


            if (type is ParameterizedType)
                return type.FullName +
                       "<" +
                       string.Join(",", (type as ParameterizedType).TypeArguments.Select(x => x.GetOriginalFullName()))
                       + ">";

            var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");
            var reference = new System.CodeDom.CodeTypeReference(type.ToString());

            return provider.GetTypeOutput(reference)
                //In some cases generic parameters are coming back wrapped in square brackets
                //and with other odd characters
                //Unsure why at the moment.
                           .Replace("<>", "")
                           .Replace("[[", "<")
                           .Replace("[[", "<")
                           .Replace("]]", ">")
                           .Replace("[", "")
                           .Replace("]", "");
        }

        /// <summary>
        /// Contains better formatting and handling of edge cases
        /// than calling <see cref="IType.FullName"/> directly
        /// </summary>
        public static string GetFullName(this IType type)
        {
            Ensure.ArgumentNotNull(type, "type");

            var astType = type.ToAstSyntaxType();

            if (astType is PrimitiveType)
                return (astType as PrimitiveType).Keyword;

            return type.FullName;
        }

        /// <summary>
        /// Correctly formats the <see cref="IType.FullName"/>
        /// so the type can be found by the .NET Type System.
        /// </summary>
        /// <remarks>
        /// Provides special handling for nested types so the 
        /// type name is Name.Space.Parent+Child instead of
        /// Name.Space.Parent.Child (which would be output by
        /// <see cref="GetOriginalFullName(ICSharpCode.NRefactory.TypeSystem.IType)"/>
        /// </remarks>
        /// <remarks>
        /// TODO: This method only supports one level of class nesting.
        /// </remarks>
        public static string GetFullTypeName(this IType type)
        {
            Ensure.ArgumentNotNull(type, "type");

            return (null == type.DeclaringType)
                       ? type.GetFullName()
                       : "global::" + type.GetFullName().ReplaceLastInstanceOf(".", "+");
        }

        /// <summary>
        /// Returns <see cref="IType.Name"/> formatted
        /// so it can be used as a variable or namespace
        /// identifier.
        /// </summary>
        public static string GetNameAsIdentifier(this IType type)
        {
            return type.GetOriginalFullName()
                .Replace(type.Namespace + ".", "")
                .AsIdentifier();
        }

        public static string GetFullNameAsIdentifier(this IType type)
        {
            return type.GetOriginalFullName()
                .AsIdentifier();
        }

        private static string AsIdentifier(this string s)
        {
            return
                (s ?? "")
                .Replace(".", "_")
                .Replace("<", "__")
                .Replace(",", "_")
                .Replace(">", "__")
                .EnsureIsShorterThan(260);
        }


        public static bool Implements(this IType type, string baseTypeFullName)
        {
            return type.GetAllBaseTypes().Any(baseType => baseType.GetOriginalFullName() == baseTypeFullName);
        }

        public static bool Implements<TBaseType>(this IType type)
        {
            return Implements(type, typeof(TBaseType).GetOriginalFullName());
        }

        public static IEnumerable<IAttribute> GetAttributes(this IType type, bool includeBaseTypes = true, bool includeNonInheritedAttributes = false)
        {
            Ensure.ArgumentNotNull(type, "type");

            IList<IAttribute> attributes = new List<IAttribute>();

            type.TryCast<DefaultResolvedTypeDefinition>(dt => attributes = dt.Attributes.ToList());

            type.TryCast<DefaultTypeParameter>(dt => attributes = dt.Attributes.ToList());

            type.TryCast<ParameterizedType>(pt => attributes = pt.GetDefinition().GetAttributes().ToList());

            //TODO: Can I get attributes on any other type?

            if (includeBaseTypes)
            {
                var baseTypeAttributes =
                    (type.GetAllBaseTypes() ?? new IType[]{})
                        .Where(bt => bt.FullName != type.FullName && !bt.IsKnownType(KnownTypeCode.Object))
                        .SelectMany(baseType => baseType.GetAttributes(true, includeNonInheritedAttributes));

                if (!includeNonInheritedAttributes)
                    baseTypeAttributes = baseTypeAttributes.FilterOutNonInheritedAttributes();

                attributes.AddRange(baseTypeAttributes);
            }

            return attributes;
        }

        public static bool IsDecoratedWithAttribute(this IType type, IType attributeType, bool includeBaseTypes = true)
        {
            return type.GetAttributes(includeBaseTypes).Any(a => a.AttributeType.Equals(attributeType));
        }

        public static bool IsNullOrUnknown(this IType type)
        {
            return (null == type || type.IsUnknown());
        }

        public static bool IsUnknown(this IType type)
        {
            return type.Kind == TypeKind.Unknown;
        }

        public static bool IsStaticOrSealed(this IType type)
        {
            var definition = type.GetDefinition();

            return (definition.IsSealed || definition.IsStatic);
        }

        public static IType ToIType(this Type type, ICompilation compilation)
        {
            return compilation.FindType(type);
        }

        /// <summary>
        /// Prints a generic type the way it would look in source code.
        /// Includes the namespace.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/6402864/c-pretty-type-name-function
        /// </remarks>
        public static string GetOriginalFullName(this Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");
            var reference = new System.CodeDom.CodeTypeReference(type.FullName);

            return provider.GetTypeOutput(reference);
        }

        public static bool HasParameterlessConstructor(this IType type)
        {
            return 
                type.GetConstructors().Any(
                    c => null == c.Parameters || 0 == c.Parameters.Count);
        }
    }
}
