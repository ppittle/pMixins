//----------------------------------------------------------------------- 
// <copyright file="GenerateMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 12:04:28 AM</date> 
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
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using log4net;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.TargetPartialClassGenerator
{
    /// <summary>
    /// Iterates through members in <see cref="pMixinGeneratorPipelineState.CurrentMixinMembers"/>
    /// and copies the members to the Target's code-behind.  Member invocation is 
    /// proxied to the Master Wrapper (created in <see cref="GenerateMixinMasterWrapperClass"/>
    /// via the __mixins private Property added in <see cref="GenerateMixinsContainerClass"/>
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public global::System.String OtherMixinMethod ()
	///	{
	///		return ___mixins.Other_Namespace_OtherMixin.OtherMixinMethod ();
	///	}
    /// ]]>
    /// </code>
    /// </example>
    /// TODO: Handle when to implement methods implicitly
    public class GenerateMembersInGeneratedClass : IPipelineStep<pMixinGeneratorPipelineState>
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            IAssembly targetParentAssembly = null;

            try
            {
                var resolveResult =
                    manager.BaseState.CommonState.Context.TypeResolver.Resolve(manager.SourceClass);

                if (!resolveResult.IsError && null != resolveResult.Type)
                    targetParentAssembly = resolveResult.Type.GetDefinition().ParentAssembly;

            }
            catch (Exception e)
            {
                _log.Warn("Exception while trying to resolve the Parent Assembly of Target class [" + manager.SourceClass.Name + "]: " + e.Message, e);
            }

            var mixinParentAssembly = manager.CurrentpMixinAttribute.Mixin.GetDefinition().ParentAssembly;

            var importMixinInternalMethods =
                (null == targetParentAssembly || null == mixinParentAssembly)
                    ? (null == mixinParentAssembly && null == targetParentAssembly)
                    : mixinParentAssembly.FullAssemblyName.Equals(targetParentAssembly.FullAssemblyName);

           var memberList =
                manager.CurrentMixinMembers.Select(x => x.Member)
                    .DistinctMembers()
                    .Where(member => (!member.IsInternal || importMixinInternalMethods));

            foreach (var member in memberList)
            {
                EntityDeclaration newMemberDeclaration = null;

                if (member is IMethod)
                    newMemberDeclaration = GenerateMethod(manager, member as IMethod);

                else if (member is IProperty)
                    newMemberDeclaration = GenerateProperty(manager, member as IProperty);

                else if (member is IField)
                    newMemberDeclaration = GenerateField(manager, member as IField);
                else
                    continue;

                //Copy attributes from member to newMemberDeclaration
                newMemberDeclaration.Attributes.AddRange(
                    member.MemberDefinition.Attributes
                        .FilterOutNonInheritedAttributes()
                        .ConvertToAttributeAstTypes()
                        .Select(attributeAstType => new AttributeSection(attributeAstType)));
            }
            
            return true;
        }

        #region Methods

        private EntityDeclaration GenerateMethod(pMixinGeneratorPipelineState manager, IMethod method)
        {
            return
                manager.GeneratedClass.CreateMethod(
                    method.GetModifiersString(),
                    method.ReturnType.GetOriginalFullNameWithGlobal(),
                    method.GetOriginalName(),
                    method.Parameters.ToKeyValuePair(),
                    GetMethodBodyStatement(method, manager),
                    method.GetGenericMethodConstraints(manager.BaseState.CommonState.Context.TypeResolver.Compilation));
        }

        private string GetMethodBodyStatement(IMethod method, pMixinGeneratorPipelineState manager)
        {
            var returnString = (method.ReturnType.Kind == TypeKind.Void)
                ? ""
                : "return";

            return string.Format("{0} {1}.{2}({3});",
                returnString,

                (method.IsStatic) 
                    ? manager.CurrentMixinMasterWrapperFullTypeName
                    : manager.CurrentMixinInstanceVariableAccessor,
                method.GetOriginalName(),
                string.Join(",", method.Parameters.Select(x => x.Name)));
        }

        #endregion

        #region Properties

        private EntityDeclaration GenerateProperty(pMixinGeneratorPipelineState manager, IProperty property)
        {
            return
                manager.GeneratedClass.CreateProperty(
                    property.GetModifiersString(),
                    property.ReturnType.GetOriginalFullNameWithGlobal(),
                    property.Name,
                    GetPropertyGetterStatement(property, manager),
                    GetPropertySetterStatement(property, manager));
        }

        private string GetPropertyGetterStatement(IProperty prop,
           pMixinGeneratorPipelineState manager)
        {
            if (!prop.CanGet || prop.Getter.IsPrivate)
                return string.Empty;

            var formatString =
                (prop.IsVirtual)
                    // virtual properties are wrapped as functions and need to be called as a method
                    ? "get{{ return {0}.{1}FuncGet(); }}"
                    : "get{{ return {0}.{1}; }}";
                    

            return string.Format(formatString,
                                 manager.CurrentMixinInstanceVariableAccessor,
                                 prop.Name);

        }

        private string GetPropertySetterStatement(IProperty prop, pMixinGeneratorPipelineState manager)
        {
            if (!prop.CanSet || prop.Setter.IsPrivate)
                return string.Empty;

            var formatString =
                (prop.IsVirtual)
                    // virtual properties are wrapped as functions and need to be called as a method
                    ? "set{{ {0}.{1}FuncSet(value); }}"
                    : "set{{ {0}.{1} = value; }}";

            return string.Format(formatString,
                                 manager.CurrentMixinInstanceVariableAccessor,
                                 prop.Name);
        }

        #endregion

        #region Fields

        private EntityDeclaration GenerateField(pMixinGeneratorPipelineState manager, IField field)
        {
            return 
                manager.GeneratedClass.CreateProperty(
                    field.GetModifiersString(),
                    field.ReturnType.GetOriginalFullNameWithGlobal(),
                    field.Name,
                    GetFieldGetterStatement(field, manager),
                    GetFieldSetterStatement(field, manager));
        }

        private string GetFieldGetterStatement(IField field, pMixinGeneratorPipelineState manager)
        {
            return string.Format("get{{ return {0}.{1}; }}",
                                (field.IsConst)
                                    ? manager.CurrentMixinMasterWrapperFullTypeName 
                                    : manager.CurrentMixinInstanceVariableAccessor,
                                field.Name);

        }

        private string GetFieldSetterStatement(IField field, pMixinGeneratorPipelineState manager)
        {
            return (field.IsConst || field.IsReadOnly)
                ? string.Empty
                : string.Format("set{{ {0}.{1} = value; }}",
                                 manager.CurrentMixinInstanceVariableAccessor,
                                 field.Name);
        }

        #endregion
    }
}
