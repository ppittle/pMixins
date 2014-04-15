//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorProxy.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 2:29:04 PM</date> 
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
using System.Linq;
using System.Text.RegularExpressions;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using CopaceticSoftware.pMixins.Infrastructure;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure
{
    public interface ICodeGeneratorProxy : IpMixinCodeGeneratorProxy
    {
        new TypeDeclaration GeneratedClassSyntaxTree { get; }
        ICodeGeneratorProxy AddNestedType(TypeDeclaration nestedType);

        new EntityDeclaration CreateMethod(string modifier, string returnTypeFullName, string methodName,
            IList<KeyValuePair<string, string>> parameters, string methodBody, string constraingClause = "",
            bool addDebuggerStepThroughAttribute = true);

        new EntityDeclaration
            CreateDataMember(string modifiers, string dataMemberTypeFullName, string dataMemberName,
                string initializerExpression = "");

        new EntityDeclaration CreateProperty(string modifier, string returnTypeFullName,
            string propertyName, string getterMethodBody, string setterMethodBody);

    }

    public class CodeGeneratorProxy : ICodeGeneratorProxy
    {
        /// <summary>
        /// Data Member Variable Names used in the generated class.
        /// Key is dataMemberTypeFullName and Value is a collection 
        /// of variable names.
        /// </summary>
        private readonly Dictionary<string, List<string>> _dataMemberVariableNames =
            new Dictionary<string, List<string>>();

        public CSharpParser CSharpParser { get; set; }

        private AttributeSection BuildCodeGeneratedAttributeSection()
        {
            var attribute = new Attribute
            {
                Type = new SimpleType("System.CodeDom.Compiler.GeneratedCodeAttribute"),
            };

            attribute.Arguments.Add(new PrimitiveExpression("pMixin"));
            attribute.Arguments.Add(new PrimitiveExpression(
                FileVersionInfo.GetVersionInfo(
                    typeof(CodeGeneratorProxy).Assembly.Location)
                .FileVersion));

            return new AttributeSection(attribute);
        }


        public CodeGeneratorProxy(TypeDeclaration generatedClass, string sourceCodeFileContents)
        {
            Ensure.ArgumentNotNull(generatedClass, "generatedClass");
            
            SourceCodeFileContents = sourceCodeFileContents;
            GeneratedClassSyntaxTree = generatedClass;
            
            CSharpParser = new CSharpParser();

            generatedClass.Attributes.Add(BuildCodeGeneratedAttributeSection());
        }

        /// <summary>
        /// Gets or Creates a valid variable name for the passed
        /// <paramref name="dataMemberTypeFullName"/>
        /// </summary>
        /// <param name="dataMemberTypeFullName">The <see cref="Type.FullName"/>
        /// of the Data Member of the variable being created.  This will
        /// be used as the basis to generate the variable name
        /// </param>
        /// <param name="forceCreate">If <c>True</c> creates a new 
        /// variable even if one already exists for <paramref name="dataMemberTypeFullName"/>
        /// </param>
        /// <returns>A valid variable name.</returns>
        public string GetOrCreateDataMemberVariableName(string dataMemberTypeFullName, bool forceCreate = false)
        {
            if (_dataMemberVariableNames.ContainsKey(dataMemberTypeFullName) && !forceCreate)
                return _dataMemberVariableNames[dataMemberTypeFullName].First();

            var newVariableName =
                    "__" + //Give a prefix to help keep it out of Intellisense
                    RemoveNamespace(dataMemberTypeFullName)
                    .EnsureIsShorterThan(500) // C# specs
                    .Replace("`", "")
                    .Replace("[", "_")
                    .Replace("]", "_")
                    .Replace("<", "_")
                    .Replace(">", "_")
                    .Replace(",", "")
                    .Replace(".", "")
                    .Replace(" ", "");

            if (_dataMemberVariableNames.ContainsKey(dataMemberTypeFullName))
            {
                var countSuffix = _dataMemberVariableNames[dataMemberTypeFullName].Count;

                if (countSuffix > 0)
                    newVariableName += countSuffix;

                _dataMemberVariableNames[dataMemberTypeFullName].Add(newVariableName);
            }
            else
            {
                _dataMemberVariableNames.Add(dataMemberTypeFullName, new List<string> { newVariableName });
            }

            return newVariableName;
        }

        /// <summary>
        /// Removes the <see cref="Type.Namespace"/>
        /// from a <see cref="Type.FullName"/>.
        /// </summary>
        public string RemoveNamespace(string typeFullName)
        {
            //http://stackoverflow.com/questions/16714064/regex-method-to-remove-namespace-from-a-type-fullname-c-sharp/16714211#16714211
            return Regex.Replace(typeFullName, @"[.\w]+\.(\w+)", "$1");
        }

        public void CreateConstructor(string modifiers, IList<KeyValuePair<string, string>> parameters, string constructorInitializer, string constructorBody)
        {
            constructorInitializer = constructorInitializer ?? "";

            constructorBody = (constructorBody ?? "")
                            .EnsureStartsWith("{").EnsureEndsWith("}");
            
            var constructorSource =
                string.Format("{0} {1} ({2}) {3} {4} ",
                              modifiers,
                              GeneratedClassSyntaxTree.Name,
                              string.Join(",", parameters.Select(x => x.Key + " " + x.Value)),
                              constructorInitializer,
                              constructorBody);

            var dummyClassWrapper = string.Format("public class {0}{{ {1} }}",
                GeneratedClassSyntaxTree.Name,
                constructorSource);

            var parsedConstructor =
                CSharpParser.Parse(dummyClassWrapper)
                .Descendants
                .OfType<ConstructorDeclaration>()
                .FirstOrDefault();

            if (null == parsedConstructor)
                #region Throw Exception
                throw new Exception(string.Format(Strings.ExceptionParsingCodeInCodeGeneratorProxy,
                    "Constructor", constructorSource));
                #endregion

            GeneratedClassSyntaxTree.AddChild(
                (ConstructorDeclaration)parsedConstructor.Clone(),
                Roles.TypeMemberRole);
        }

        public bool ContainsDataMember(string dataMemberName)
        {
            Ensure.ArgumentNotNullOrEmpty(dataMemberName, "dataMemberName");

            return GeneratedClassSyntaxTree.Descendants.OfType<FieldDeclaration>()
                //Only check the highest level type declaration
                .Where(m => m.GetParent<TypeDeclaration>() == null)
                .Any(fd =>
                    fd.Descendants.OfType<VariableInitializer>().Any(vi =>
                        vi.Name == dataMemberName));
        }

        public EntityDeclaration CreateDataMember(string modifiers, string dataMemberTypeFullName, string dataMemberName, string initializerExpression = "")
        {
            Ensure.ArgumentNotNull(dataMemberTypeFullName, "dataMemberTypeFullName");
            Ensure.ArgumentNotNullOrEmpty(dataMemberName, "dataMemberName");

            if (ContainsDataMember(dataMemberName))
                #region Throw Exception
                throw new Exception(string.Format(
                    Strings.ExceptionCreateDataMemberFailedBecauseClassAlreadyContainsDataMemberName,
                    dataMemberName));
                #endregion

            var initializationString =
                string.IsNullOrEmpty(initializerExpression)
                    ? ";"
                    : initializerExpression.Trim().EnsureStartsWith("=").EnsureEndsWith(";");

            var parsedDataMemberSource = string.Format("{0} {1} {2} {3}",
                                                       modifiers,
                                                       dataMemberTypeFullName,
                                                       dataMemberName,
                                                       initializationString);

            var parsedDataMember = CSharpParser.ParseTypeMembers(parsedDataMemberSource)
                .FirstOrDefault()
                as FieldDeclaration;

            if (null == parsedDataMember)
                #region Throw Exception
                throw new Exception(string.Format(Strings.ExceptionParsingCodeInCodeGeneratorProxy,
                    "Data Member", parsedDataMemberSource));
                #endregion

            GeneratedClassSyntaxTree.AddChild(parsedDataMember, Roles.TypeMemberRole);

            return parsedDataMember;
        }

        public bool ContainsMethod(string methodName, IList<KeyValuePair<string, string>> parameters)
        {
            Ensure.ArgumentNotNullOrEmpty(methodName, "methodName");
            Ensure.ArgumentNotNull(parameters, "parameters");

            return GeneratedClassSyntaxTree.Descendants
                .OfType<MethodDeclaration>()
                //Only check the highest level type declaration
                .Where(m => m.GetParent<TypeDeclaration>() == null)
                .Any(x => x.Name == methodName
                          && x.Parameters.Count == parameters.Count()
                          && x.Parameters.All(p => parameters.Any(
                              matchingP => p.Type.GetName() == matchingP.Key)));
        }

        public EntityDeclaration CreateMethod(string modifier, string returnTypeFullName, string methodName,
                                 IList<KeyValuePair<string, string>> parameters, string methodBody, string constraingClause = "", bool addDebuggerStepThroughAttribute = true)
        {
            Ensure.ArgumentNotNullOrEmpty(returnTypeFullName, "returnTypeFullName");
            Ensure.ArgumentNotNullOrEmpty(methodName, "methodName");
            Ensure.ArgumentNotNull(parameters, "parameters");

            if (ContainsMethod(methodName, parameters))
                #region Throw Exception
                throw new Exception(string.Format(
                    Strings.ExceptionCreateMethodFailedBecauseClassAlreadyContainsMethodSignature,
                    methodName,
                    string.Join(",", parameters.Select(x => x.Key + x.Value))));
                #endregion

            methodBody =
                methodBody.IsNullOrEmpty()
                    ? ";"
                    : methodBody.EnsureStartsWith("{").EnsureEndsWith("}");

            var debuggerStepThroughAttribute = (addDebuggerStepThroughAttribute)
                                                   ? "[global::System.Diagnostics.DebuggerStepThrough]"
                                                   : "";

            var methodSource =
                string.Format("{0} {1} {2} {3}({4}) {5} {6} ",
                              debuggerStepThroughAttribute,
                              modifier,
                              returnTypeFullName,
                              methodName,
                              string.Join(",", parameters.Select(x => x.Key + " " + x.Value)),
                              constraingClause,
                              methodBody);

            var parsedMethod = CSharpParser.ParseTypeMembers(methodSource).FirstOrDefault();

            if (null == parsedMethod)
                #region Throw Exception
                throw new Exception(string.Format(Strings.ExceptionParsingCodeInCodeGeneratorProxy,
                    "Method", methodSource));
                #endregion
            
            GeneratedClassSyntaxTree.AddChild(parsedMethod, Roles.TypeMemberRole);

            return parsedMethod;
        }
        
        public bool ContainsProperty(string propertyName)
        {
            Ensure.ArgumentNotNullOrEmpty(propertyName, "propertyName");

            return GeneratedClassSyntaxTree.Descendants
                .OfType<PropertyDeclaration>()
                //Only check the highest level type declaration
                .Where(m => m.GetParent<TypeDeclaration>() == null)
                .Any(p =>
                     p.Name == propertyName);
        }

        public EntityDeclaration CreateProperty(string modifier, string returnTypeFullName,
            string propertyName, string getterMethodBody, string setterMethodBody)
        {

            Ensure.ArgumentNotNullOrEmpty(returnTypeFullName, "returnTypeFullName");
            Ensure.ArgumentNotNullOrEmpty(propertyName, "propertyName");

            var propSource =
                string.Format("{0} {1} {2} {{ {3} {4} }}",
                              modifier,
                              returnTypeFullName,
                              propertyName,
                              getterMethodBody,
                              setterMethodBody);

            var parsedProperty = CSharpParser.ParseTypeMembers(propSource).FirstOrDefault();

            if (null == parsedProperty)
                #region Throw Exception
                throw new Exception(string.Format(Strings.ExceptionParsingCodeInCodeGeneratorProxy,
                    "Property", propSource));
                #endregion

            GeneratedClassSyntaxTree.AddChild(parsedProperty, Roles.TypeMemberRole);

            return parsedProperty;
        }

        public void ImplementInterface(string interfaceFullName)
        {
            if (GeneratedClassSyntaxTree.Descendants.OfType<SimpleType>()
                                 .Any(x => x.Identifier == interfaceFullName))
                return;

            GeneratedClassSyntaxTree.AddChild(
                new SimpleType(interfaceFullName),
                Roles.BaseType);
        }

        public IpMixinCodeGeneratorProxy CreateNestedType(string nestTypeSource)
        {
            Ensure.ArgumentNotNullOrEmpty(nestTypeSource, "nestTypeSource");

            var parsedNestedType = CSharpParser.Parse(nestTypeSource).FirstChild
                as TypeDeclaration;

            if (null == parsedNestedType)
                #region Throw Exception
                throw new Exception(string.Format(Strings.ExceptionParsingCodeInCodeGeneratorProxy,
                    "Nested Type", nestTypeSource));
                #endregion

            return AddNestedType((TypeDeclaration) parsedNestedType.Clone());
        }

        public ICodeGeneratorProxy AddNestedType(TypeDeclaration nestedType)
        {
            GeneratedClassSyntaxTree.AddChildTypeDeclaration(nestedType);

            return new CodeGeneratorProxy(nestedType, "");
        }

        public bool ContainsNestedType(string nestedTypeName)
        {
            return GeneratedClassSyntaxTree.Descendants
                                           .OfType<TypeDeclaration>()
                                           .Any(x => x.Name == nestedTypeName);
        }

        public string ClassName { get { return GeneratedClassSyntaxTree.Name; } }

        public void AddUsingStatement(string @namespace)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
                return;

            if (GeneratedClassSyntaxTree
                .Descendants
                .OfType<UsingDeclaration>()
                .Any(x => x.GetText().Replace("using ", "").Replace(";", "").Equals(@namespace)))
            {
                return;
            }

            GeneratedClassSyntaxTree.AddChild(
                new UsingDeclaration(@namespace),
                new Role<UsingDeclaration>("UsingDeclaration"));
        }

        object IpMixinCodeGeneratorProxy.GeneratedClassSyntaxTree { get { return GeneratedClassSyntaxTree; } }
        public TypeDeclaration GeneratedClassSyntaxTree { get; private set; }
        public string SourceCodeFileContents { get; private set; }

        object IpMixinCodeGeneratorProxy.CreateMethod(string modifier, string returnTypeFullName, string methodName,
            IList<KeyValuePair<string, string>> parameters, string methodBody, string constraintClause,
            bool addDebuggerStepThroughAttribute)
        {
            return CreateMethod(modifier, returnTypeFullName, methodName, parameters, methodBody, constraintClause,
                addDebuggerStepThroughAttribute);
        }

        object IpMixinCodeGeneratorProxy.
            CreateDataMember(string modifiers, string dataMemberTypeFullName, string dataMemberName,
                string initializerExpression)
        {
            return CreateDataMember(modifiers, dataMemberTypeFullName, dataMemberName, initializerExpression);
        }

        object IpMixinCodeGeneratorProxy.CreateProperty(string modifier, string returnTypeFullName,
            string propertyName, string getterMethodBody, string setterMethodBody)
        {
            return CreateProperty(modifier, returnTypeFullName, propertyName, getterMethodBody, setterMethodBody);
        }
    }
}
