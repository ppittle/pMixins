//----------------------------------------------------------------------- 
// <copyright file="GenerateMembersInTargetClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, July 26, 2014 1:26:43 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds the <see cref="MixinGenerationPlan.MembersPromotedToTarget"/> to the 
    /// <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>.
    /// These methods will effectively be available to consumers of Target.
    /// 
    /// Member invocation is 
    /// proxied to the Master Wrapper (<see cref="MixinLevelCodeGeneratorPipelineState.MasterWrapper"/>
    /// via the __mixins private Property (<see cref="TargetCodeBehindPlan.MixinsPropertyName"/>)
    /// <code>
    /// <![CDATA[
    /// public partial class Target
    /// {
    ///     public void MixinMethod()
    ///     {
    ///         ___mixins.Other_Namespace_OtherMixin.OtherMixinMethod (); 
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateMembersInTargetClass : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            var targetCodeBehindCodeGeneratorProxy = 
                new CodeGeneratorProxy(
                    manager.TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration);

            GenerateMembers(
                manager,
                targetCodeBehindCodeGeneratorProxy,
                manager.MixinGenerationPlan.MembersPromotedToTarget);

            return true;
        }

        private void GenerateMembers(
            MixinLevelCodeGeneratorPipelineState manager,
            ICodeGeneratorProxy targetCodeBehind, 
            IEnumerable<MemberWrapper> membersPromotedToTarget)
        {

            var compilation =
                manager.CommonState.Context.TypeResolver.Compilation;

            //__mixins.Test_ExampleMixin
            var masterWrapperVariableName =
                manager.MixinGenerationPlan.MasterWrapperPlan
                    .MasterWrapperInstanceNameAvailableFromTargetCodeBehind;

            var masterWrapperStaticName =
                manager.MasterWrapper.GetFullTypeName();


            foreach (var mw in membersPromotedToTarget)
            {
                EntityDeclaration newMemberDeclaration = null;

                if (mw.Member is IMethod)
                    newMemberDeclaration = GenerateMethod(
                        mw,
                        targetCodeBehind,
                        compilation,
                        mw.Member.IsStaticOrConst()
                            ? masterWrapperStaticName
                            : masterWrapperVariableName);

                else if (mw.Member is IProperty)
                    newMemberDeclaration = GenerateProperty(
                        mw,
                        targetCodeBehind,
                        mw.Member.IsStaticOrConst()
                            ? masterWrapperStaticName
                            : masterWrapperVariableName);

                else if (mw.Member is IField)
                    newMemberDeclaration = GenerateField(
                        mw.Member as IField,
                        targetCodeBehind,
                        mw.Member.IsStaticOrConst()
                            ? masterWrapperStaticName
                            : masterWrapperVariableName);

                else
                    continue;

                //Copy attributes from member to newMemberDeclaration
                newMemberDeclaration.Attributes.AddRange(
                    mw.Member.MemberDefinition.Attributes
                        .FilterOutNonInheritedAttributes()
                        .ConvertToAttributeAstTypes()
                        .Select(attributeAstType => new AttributeSection(attributeAstType)));
            }
            
        }
        
        #region Methods

        private EntityDeclaration GenerateMethod(
            MemberWrapper mw,
            ICodeGeneratorProxy targetCodeBehind,
            ICompilation compilation,
            string masterWrapperVariableName)
        {
            var method = (IMethod) mw.Member;

            if (mw.ImplementationDetails.ImplementExplicitly)
            {
                return 
                    targetCodeBehind.CreateMethod(
                        modifier:
                            string.Empty,
                        returnTypeFullName:
                            method.ReturnType.GetOriginalFullNameWithGlobal(),
                        methodName:
                            mw.ImplementationDetails.ExplicitInterfaceImplementationType
                                .GetOriginalFullNameWithGlobal() +
                                "." + method.Name,
                        parameters:
                            method.Parameters.ToKeyValuePair(),
                        methodBody:
                            string.Format(
                                "{0} (({1}){2}).{3}({4});",
                            (mw.Member.ReturnType.Kind == TypeKind.Void)
                                ? string.Empty : "return",

                            mw.ImplementationDetails.ExplicitInterfaceImplementationType
                                .GetOriginalFullNameWithGlobal(),
                            masterWrapperVariableName,

                            method.GetOriginalName(),

                            string.Join(",", method.Parameters.Select(x => x.Name))),
                    constraingClause:
                        method.GetGenericMethodConstraints(compilation));
            }
            else
            {
                return
                    targetCodeBehind.CreateMethod(
                        modifier:
                            method.GetModifiersString(
                                overrideModifiers: 
                                    mw.ImplementationDetails.ImplementInTargetAsAbstract 
                                    ? "abstract" : null),
                        returnTypeFullName:
                            method.ReturnType.GetOriginalFullNameWithGlobal(),
                        methodName:
                            method.GetOriginalName(),
                        parameters:
                            method.Parameters.ToKeyValuePair(),
                        methodBody:
                            mw.ImplementationDetails.ImplementInTargetAsAbstract
                                ? string.Empty
                                : string.Format(
                                    "{0} {1}.{2}({3});",
                                    (method.ReturnType.Kind == TypeKind.Void)
                                        ? string.Empty
                                        : "return",
                                    masterWrapperVariableName,
                                    method.GetOriginalName(),
                                    string.Join(",", method.Parameters.Select(x => x.Name))),
                        constraingClause:
                            method.GetGenericMethodConstraints(compilation));
            }
        }
        #endregion

        #region Properties

        private EntityDeclaration GenerateProperty(
            MemberWrapper mw,
            ICodeGeneratorProxy targetCodeBehind,
            string masterWrapperVariableName)
        {
            var property = (IProperty) mw.Member;

            if (mw.ImplementationDetails.ImplementExplicitly)
            {
                return
                    targetCodeBehind.CreateProperty(
                        modifier:
                            string.Empty,
                        returnTypeFullName:
                            property.ReturnType.GetOriginalFullNameWithGlobal(),
                        propertyName:
                            property.IsIndexer
                                ? string.Format(
                                    "{0}.this [{1} {2}]",
                                    mw.ImplementationDetails.ExplicitInterfaceImplementationType
                                        .GetOriginalFullNameWithGlobal(),
                                    property.Parameters.First().Type.GetOriginalFullNameWithGlobal(),
                                    property.Parameters.First().Name
                                    )
                                :
                                mw.ImplementationDetails.ExplicitInterfaceImplementationType
                                    .GetOriginalFullNameWithGlobal() +
                                 "." + property.Name,
                        getterMethodBody:
                            GetPropertyGetterStatement(mw, masterWrapperVariableName),
                        setterMethodBody:
                            GetPropertySetterStatement(mw,  masterWrapperVariableName));
            }
            else
            {
                return
                    targetCodeBehind.CreateProperty(
                        modifier:
                            property.GetModifiersString(
                                overrideModifiers: 
                                    mw.ImplementationDetails.ImplementInTargetAsAbstract
                                    ? "abstract" : null),
                        returnTypeFullName:
                            property.ReturnType.GetOriginalFullNameWithGlobal(),
                        propertyName:
                            property.IsIndexer
                                ? string.Format(
                                    "this [{0} {1}]",
                                    property.Parameters.First().Type.GetOriginalFullNameWithGlobal(),
                                    property.Parameters.First().Name
                                    )
                                : property.Name,
                        getterMethodBody:
                            GetPropertyGetterStatement(mw, masterWrapperVariableName),
                        setterMethodBody:
                            GetPropertySetterStatement(mw,masterWrapperVariableName));
            }
        }

        private string GetPropertyGetterStatement(
            MemberWrapper mw,
            string masterWrapperVariableName)
        {
            var prop = (IProperty) mw.Member;

            if (!prop.CanGet || prop.Getter.IsPrivate)
                return string.Empty;

            if (mw.ImplementationDetails.ImplementInTargetAsAbstract)
                return "get;";

            if (mw.ImplementationDetails.ImplementExplicitly)
                return string.Format(
                    "get{{ return (({0}){1}){2}; }}",
                    mw.ImplementationDetails.ExplicitInterfaceImplementationType
                        .GetOriginalFullNameWithGlobal(),
                    masterWrapperVariableName,
                    prop.IsIndexer
                        ? "[" + prop.Parameters.First().Name + "]"
                        : "." + prop.Name);
            

            return string.Format(
                "get{{ return {0}{1}; }}",
                masterWrapperVariableName,
                prop.IsIndexer
                 ? "[" + prop.Parameters.First().Name + "]"
                 : "." + prop.Name);
        }

        private string GetPropertySetterStatement(
            MemberWrapper mw,
            string masterWrapperVariableName)
        {
            var prop = (IProperty)mw.Member;

            if (!prop.CanSet || prop.Setter.IsPrivate)
                return string.Empty;

            if (mw.ImplementationDetails.ImplementInTargetAsAbstract)
                return "set;";

            if (mw.ImplementationDetails.ImplementExplicitly)
                return string.Format(
                    "set{{ return (({0}){1}){2} = value; }}",
                    mw.ImplementationDetails.ExplicitInterfaceImplementationType
                        .GetOriginalFullNameWithGlobal(),
                    masterWrapperVariableName,
                    prop.IsIndexer
                        ? "[" + prop.Parameters.First().Name + "]"
                        : "." + prop.Name);


            return string.Format(
                "set{{ {0}{1} = value; }}",
                masterWrapperVariableName,
                prop.IsIndexer
                 ? "[" + prop.Parameters.First().Name + "]"
                 : "." + prop.Name);
        }

        #endregion

        #region Fields

        private EntityDeclaration GenerateField(
            IField field,
            ICodeGeneratorProxy targetCodeBehind,
            string masterWrapperVariableName)
        {
            return

                targetCodeBehind.CreateProperty(
                    modifier:
                        field.GetModifiersString(),
                    returnTypeFullName:
                        field.ReturnType.GetOriginalFullNameWithGlobal(),
                    propertyName:
                        field.Name,
                    getterMethodBody:
                        string.Format(
                            "get{{ return {0}.{1}; }}",
                            masterWrapperVariableName,
                            field.Name),
                    setterMethodBody:
                        (field.IsConst || field.IsReadOnly)
                            ? string.Empty
                            : string.Format(
                                "set{{ {0}.{1} = value; }}",
                                masterWrapperVariableName,
                                field.Name));

        }
        #endregion
    }
}
