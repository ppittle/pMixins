//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorProxyHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 2, 2014 7:10:32 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure
{
    public class CodeGeneratorProxyMemberHelper
    {
        protected readonly ICodeGeneratorProxy CodeGeneratorProxy;
        protected readonly ICompilation Compilation;
        public CodeGeneratorProxyMemberHelper(ICodeGeneratorProxy codeGeneratorProxy, ICompilation compilation)
        {
            CodeGeneratorProxy = codeGeneratorProxy;
            Compilation = compilation;
        }


        public string GetMethodBodyStatement(IMethod method, Func<IMember, string> baseObjectIdentifierFunc = null, 
            Func<IMember, string> methodNameFunc = null)
        {
            if (null == methodNameFunc)
                methodNameFunc = member => member.GetOriginalName();

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = member => "base";

            return GetMethodBodyStatementImpl(method, baseObjectIdentifierFunc, methodNameFunc);
        }

        protected virtual string GetMethodBodyStatementImpl(IMethod method,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> methodNameFunc)
        {
            return string.Format("{0} {1}.{2}({3});",
                                method.GetReturnString(),
                                baseObjectIdentifierFunc(method),
                                methodNameFunc(method),
                                string.Join(",", method.Parameters.Select(x => x.Name)));
        }

        public string GetPropertyGetterStatement(IProperty prop, Func<IMember, string> baseObjectIdentifierFunc = null,
            Func<IMember, string> propertyNameFunc = null)
        {
            if (null == propertyNameFunc)
                propertyNameFunc = member => member.GetOriginalName();

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = member => "base";

            if (!prop.CanGet || prop.Getter.IsPrivate)
                return string.Empty;

            return GetPropertyGetterStatementImpl(prop, baseObjectIdentifierFunc, propertyNameFunc);
        }

        protected virtual string GetPropertyGetterStatementImpl(IProperty prop,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> propertyNameFunc )
        {
            return string.Format("get{{ return {0}.{1}; }}",
                               baseObjectIdentifierFunc(prop),
                                propertyNameFunc(prop));
        }

        public string GetPropertySetterStatement(IProperty prop, Func<IMember, string> baseObjectIdentifierFunc = null,
            Func<IMember, string> propertyNameFunc = null)
        {
            if (null == propertyNameFunc)
                propertyNameFunc = member => member.GetOriginalName();

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = member => "base";

            if (!prop.CanSet || prop.Setter.IsPrivate)
                return string.Empty;

            return GetPropertySetterStatementImpl(prop, baseObjectIdentifierFunc, propertyNameFunc);
        }

        protected virtual string GetPropertySetterStatementImpl(IProperty prop,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> propertyNameFunc)
        {
            return string.Format("set{{ {0}.{1} = value; }}",
                                 baseObjectIdentifierFunc(prop),
                                 propertyNameFunc(prop));
        }

        public virtual string GetFieldGetterStatement(IField prop, Func<IMember, string> baseObjectIdentifierFunc = null,
            Func<IMember, string> propertyNameFunc = null)
        {
            if (null == propertyNameFunc)
                propertyNameFunc = member => member.GetOriginalName();

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = member => "base";

            return string.Format("get{{ return {0}.{1}; }}",
                                 baseObjectIdentifierFunc(prop),
                                 propertyNameFunc(prop));

        }

        public virtual string GetFieldSetterStatement(IField prop, Func<IMember, string> baseObjectIdentifierFunc = null,
            Func<IMember, string> propertyNameFunc = null)
        {
            if (null == propertyNameFunc)
                propertyNameFunc = member => member.GetOriginalName();

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = member => "base";

            return
                (prop.IsConst || prop.IsReadOnly)
                ? string.Empty
                : string.Format("set{{ {0}.{1} = value; }}",
                                 baseObjectIdentifierFunc(prop),
                                 propertyNameFunc(prop));
        }

        public virtual void CreateMembers(
            IEnumerable<IMember> members,
            Func<IMember, string> generateMemberModifier = null,
            Func<IMember, string> generateReturnTypeFunc = null,
            Func<IMember, string> generateMemberNameFunc = null,
            Func<IMember, string> baseObjectIdentifierFunc = null,
            Func<IMember, string> baseObjectMemberNameFunc = null, 
            bool importSystemObjectMembers = false)
        {
            if (null == generateMemberModifier)
                generateMemberModifier = member => "public";

            if (null == generateReturnTypeFunc)
                generateReturnTypeFunc = member => member.ReturnType.GetOriginalFullNameWithGlobal();

            if (null == generateMemberNameFunc)
                generateMemberNameFunc = member => member.GetOriginalName();

            if (!importSystemObjectMembers)
                members = members.Where(member => !member.FullName.StartsWith("System.Object"));

            foreach (var member in members)
            {
                #region Process Methods
                if (member is IMethod)
                {
                    CodeGeneratorProxy.CreateMethod(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        (member as IMethod).Parameters.ToKeyValuePair(),
                        GetMethodBodyStatement(member as IMethod, baseObjectIdentifierFunc, baseObjectMemberNameFunc),
                        (member as IMethod).GetGenericMethodConstraints(Compilation));
                }
                #endregion

                #region Process Properties
                else if (member is IProperty)
                {
                    CodeGeneratorProxy.CreateProperty(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        GetPropertyGetterStatement(member as IProperty, baseObjectIdentifierFunc, baseObjectMemberNameFunc),
                        GetPropertySetterStatement(member as IProperty, baseObjectIdentifierFunc, baseObjectMemberNameFunc));
                }
                #endregion

                #region Process Fields
                else if (member is IField)
                {
                    CodeGeneratorProxy.CreateProperty(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        GetFieldGetterStatement(member as IField, baseObjectIdentifierFunc, baseObjectMemberNameFunc),
                        GetFieldSetterStatement(member as IField, baseObjectIdentifierFunc, baseObjectMemberNameFunc));
                }
                #endregion
            }

        }
    }
}
