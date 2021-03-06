﻿//----------------------------------------------------------------------- 
// <copyright file="TypeDefinitionExtensions.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static bool IsNestedType(this ITypeDefinition typeDefinition)
        {
            return typeDefinition.Name.Contains("+");
        }

        public static string GetFullTypeName(this TypeDeclaration typeDeclaration)
        {
            if (null == typeDeclaration)
                throw new ArgumentNullException("typeDeclaration");

            var parentContainerClass =
                typeDeclaration.GetParent<TypeDeclaration>();

            if (null != parentContainerClass)
                return parentContainerClass.GetFullTypeName() + "." + typeDeclaration.Name;

            var namespaceDeclaration =
                typeDeclaration.GetParent<NamespaceDeclaration>();

            return
                (
                    (null != namespaceDeclaration)
                        ? namespaceDeclaration.FullName.EnsureEndsWith(".")
                        : ""
                ) +
                typeDeclaration.Name;
        }

        public static string GetFullTypeNameWithGlobal(this TypeDeclaration typeDeclaration)
        {
            return GetFullTypeName(typeDeclaration).EnsureStartsWith("global::");
        }
    }
}
