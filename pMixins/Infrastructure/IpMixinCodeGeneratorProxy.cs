//----------------------------------------------------------------------- 
// <copyright file="IpMixinCodeGeneratorProxy.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 2:29:03 PM</date> 
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
using System.Diagnostics;
using System.Reflection;

namespace CopaceticSoftware.pMixins.Infrastructure
{
    /// <summary>
    /// A proxy/facade representation of the pMixin Code Generator
    /// which allows some extensibility for controlling how code is generated.
    /// </summary>
    /// <remarks>
    /// This is proxied to reduce the number of references needed
    /// by the pMixin assembly. 
    /// </remarks>
    public interface IpMixinCodeGeneratorProxy
    {
        /// <summary>
        /// Adds a Using statement to the class.
        /// </summary>
        void AddUsingStatement(string @namespace);

        /// <summary>
        /// Removes the <see cref="Type.Namespace"/>
        /// from a <see cref="Type.FullName"/>.
        /// </summary>
        string RemoveNamespace(string typeFullName);

        /// <summary>
        /// Gets or Creates a valid variable name for the passed
        /// <paramref name="dataMemberTypeFullName"/>
        /// </summary>
        /// <param name="dataMemberTypeFullName">The <see cref="Type.FullName"/>
        /// of the Data Member of the variable being created.  This will
        /// be used as the basis to generate the variable name
        /// </param>
        /// <param name="forceCreate">If <c>True</c> creates a new 
        /// variable even if one already exists for <paramref name="dataMemberTypeFullName"/>.
        /// Default is <c>False</c>
        /// </param>
        /// <returns>A valid variable name.</returns>
        string GetOrCreateDataMemberVariableName(string dataMemberTypeFullName,
            bool forceCreate = false);

        void CreateConstructor(string modifiers, IList<KeyValuePair<string, string>> parameters, string constructorInitializer, string constructorBody);

        /// <summary>
        /// Returns <c>True</c> if the <see cref="GeneratedClassSyntaxTree"/>
        /// contains a Data Member matching the <paramref name="dataMemberName"/>.
        /// </summary>
        bool ContainsDataMember(string dataMemberName);

        /// <summary>
        /// Creates a new data member with the signature 
        /// <paramref name="dataMemberTypeFullName"/> <paramref name="dataMemberName"/>
        /// </summary>
        /// <remarks>
        /// This method should throw an exception if the <see cref="GeneratedClassSyntaxTree"/>
        /// already contains a Data Member with a name <paramref name="dataMemberName"/>
        /// </remarks>
        /// <param name="modifiers">
        /// A collection of modifiers that will appear before <see cref="dataMemberTypeFullName"/>
        /// <example>
        /// private
        /// </example>
        /// <example>
        /// protected static
        /// </example>
        /// </param>
        /// <param name="dataMemberTypeFullName">
        /// The type of the Data Member
        /// </param>
        /// <param name="dataMemberName">
        /// The name of the Data Member
        /// </param>
        /// <param name="initializerExpression">
        /// An optional string for setting initial value of the Data Member.  Does not need
        /// to begin with an equal sign or end with a semicolon.
        /// <example>
        /// "5" or "new Object()"
        /// </example>
        /// </param> 
        object CreateDataMember(
            string modifiers, string dataMemberTypeFullName,
            string dataMemberName, string initializerExpression = "");

        bool ContainsMethod(string methodName,
                            IList<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Creates a new method.
        /// </summary>
        /// <param name="modifier">String containing 0 or more method level access modifiers.
        /// <example><see cref="string.Empty"/></example>
        /// <example>"private partial"</example>
        /// </param>
        /// <param name="returnTypeFullName">
        /// The <see cref="Type.FullName"/> returned by this method.
        /// </param>
        /// <param name="methodName">
        /// The <see cref="MemberInfo.Name"/> for this method.
        /// </param>
        /// <param name="parameters">
        /// A collection of <see cref="KeyValuePair{TKey,TValue}"/>s representing the method's parameters
        /// where the <see cref="KeyValuePair{TKey,TValue}.Key"/> is the <see cref="Type.FullName"/> of 
        /// the parameter and the <see cref="KeyValuePair{TKey,TValue}.Value"/> is the parameters name.
        /// </param>
        /// <param name="methodBody">
        /// The source code representing the method body.  
        /// <remarks>If this is <see cref="string.Empty"/> than the method will be implemented 
        /// as <paramref name="methodName"/>(params ...);  However, it will still be necessary
        /// for the caller to add the 'abstract' modifier to <paramref name="modifier"/>
        /// </remarks>
        /// </param>
        /// <param name="constraintClause">A 'where' clause that contains constraints for any generic type parameters.
        /// </param>
        /// <param name="addDebuggerStepThroughAttribute">
        /// Indicates if a <see cref="DebuggerStepThroughAttribute"/> should automatically be added to the method.
        /// </param>
        object CreateMethod(string modifier, string returnTypeFullName, string methodName,
                            IList<KeyValuePair<string, string>> parameters, string methodBody = "", string constraintClause = "", bool addDebuggerStepThroughAttribute = true);

        bool ContainsProperty(string propertyName);

        object CreateProperty(string modifier, string returnTypeFullName, string propertyName,
                            string getterMethodBody, string setterMethodBody);

        void ImplementInterface(string interfaceFullName);

        IpMixinCodeGeneratorProxy CreateNestedType(string nestTypeSource);

        bool ContainsNestedType(string nestedTypeName);

        /// <summary>
        /// Returns the name of the class (without namespace) 
        /// that's currently being worked with.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Provides access to the Abstract Syntax Tree representing the Generated Class.
        /// This is expected to be of type ICSharpCode.NRefactory.CSharp.SytnaxTree
        /// </summary>
        /// <remarks>
        /// This property is provided as an object so the pMixin assembly
        /// wont have a dependency on NRefacotry.
        /// </remarks>
        object GeneratedClassSyntaxTree { get; }

        /// <summary>
        /// The contents of the source file being compiled by the pMixin Code Generator.
        /// </summary>
        string SourceCodeFileContents { get; }
    }
}
