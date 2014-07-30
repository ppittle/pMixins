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

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy
{
    public class CodeGeneratorProxyMemberHelper
    {
        public ICodeGeneratorProxy CodeGeneratorProxy { get; protected set; }
        public ICompilation Compilation { get; protected set; }

        public CodeGeneratorProxyMemberHelper(ICodeGeneratorProxy codeGeneratorProxy, ICompilation compilation)
        {
            CodeGeneratorProxy = codeGeneratorProxy;
            Compilation = compilation;
        }

        public string GetMethodBodyStatement(
            IMethod method,
            string baseObjectIdentifier = null, 
            string methodName = null)
        {
            if (null == baseObjectIdentifier)
                baseObjectIdentifier = "base";

            if (null == methodName)
                methodName = method.GetOriginalName();

            return GetMethodBodyStatementImpl(method, baseObjectIdentifier, methodName);
        }

        protected virtual string GetMethodBodyStatementImpl(
            IMethod method,
            string baseObjectIdentifier,
            string methodName)
        {
            return string.Format("{0} {1}.{2}({3});",
                                method.GetReturnString(),
                                baseObjectIdentifier,
                                methodName,
                                string.Join(",", method.Parameters.Select(x => x.Name)));
        }

        public string GetPropertyGetterStatement(
            IProperty prop,
            string baseObjectIdentifier = null,
            string propertyName = null)
        {
            if (null == propertyName)
                propertyName = prop.GetOriginalName();

            if (null == baseObjectIdentifier)
                baseObjectIdentifier = "base";

            if (!prop.CanGet || prop.Getter.IsPrivate)
                return string.Empty;

            return GetPropertyGetterStatementImpl(prop, baseObjectIdentifier, propertyName);
        }

        protected virtual string GetPropertyGetterStatementImpl(IProperty prop,
            string baseObjectIdentifier,
            string propertyName )
        {
            return string.Format(
                "get{{ return {0}.{1}; }}",
                baseObjectIdentifier,
                propertyName);
        }

        public string GetPropertySetterStatement(
            IProperty prop, 
            string baseObjectIdentifier = null,
            string propertyName = null)
        {
            if (null == propertyName)
                propertyName = prop.GetOriginalName();

            if (null == baseObjectIdentifier)
                baseObjectIdentifier = "base";

            if (!prop.CanSet || prop.Setter.IsPrivate)
                return string.Empty;

            return GetPropertySetterStatementImpl(prop, baseObjectIdentifier, propertyName);
        }

        protected virtual string GetPropertySetterStatementImpl(IProperty prop,
            string baseObjectIdentifier,
            string propertyName)
        {
            return string.Format(
                "set{{ {0}.{1} = value; }}",
                baseObjectIdentifier,
                propertyName);
        }

        public virtual string GetFieldGetterStatement(
            IField field, 
            string baseObjectIdentifier = null,
            string propertyName = null)
        {
            if (null == propertyName)
                propertyName = field.GetOriginalName();

            if (null == baseObjectIdentifier)
                baseObjectIdentifier =  "base";

            return string.Format("get{{ return {0}.{1}; }}",
                                 baseObjectIdentifier,
                                 propertyName);

        }

        public virtual string GetFieldSetterStatement(
            IField field, 
            string baseObjectIdentifier = null,
            string propertyName = null)
        {
            if (null == propertyName)
                propertyName = field.GetOriginalName();

            if (null == baseObjectIdentifier)
                baseObjectIdentifier = "base";

            return
                (field.IsConst || field.IsReadOnly)
                ? string.Empty
                : string.Format("set{{ {0}.{1} = value; }}",
                                 baseObjectIdentifier,
                                 propertyName);
        }

        public virtual void CreateMembers(
            IEnumerable<MemberWrapper> members,
            Func<MemberWrapper, string> generateMemberModifier = null,
            Func<MemberWrapper, string> generateReturnTypeFunc = null,
            Func<MemberWrapper, string> generateMemberNameFunc = null,
            Func<MemberWrapper, string> baseObjectIdentifierFunc = null,
            Func<MemberWrapper, string> baseObjectMemberNameFunc = null, 
            bool importSystemObjectMembers = false)
        {
            if (null == generateMemberModifier)
                generateMemberModifier = member => "public";

            if (null == generateReturnTypeFunc)
                generateReturnTypeFunc = member => member.Member.ReturnType.GetOriginalFullNameWithGlobal();

            if (null == generateMemberNameFunc)
                generateMemberNameFunc = member => member.Member.GetOriginalName();

            if (!importSystemObjectMembers)
                members = members.Where(member => !member.Member.FullName.StartsWith("System.Object"));

            if (null == baseObjectIdentifierFunc)
                baseObjectIdentifierFunc = m => null;

            if (null == baseObjectMemberNameFunc)
                baseObjectMemberNameFunc = m => null;

            foreach (var member in members)
            {
                #region Process Methods
                if (member.Member is IMethod)
                {
                    CodeGeneratorProxy.CreateMethod(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        (member.Member as IMethod).Parameters.ToKeyValuePair(),
                        GetMethodBodyStatement(
                            member.Member as IMethod, 
                            baseObjectIdentifierFunc(member), 
                            baseObjectMemberNameFunc(member)),
                        (member.Member as IMethod).GetGenericMethodConstraints(Compilation));
                }
                #endregion

                #region Process Properties
                else if (member.Member is IProperty)
                {
                    CodeGeneratorProxy.CreateProperty(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        GetPropertyGetterStatement(
                            member.Member as IProperty, 
                            baseObjectIdentifierFunc(member), 
                            baseObjectMemberNameFunc(member)),
                        GetPropertySetterStatement(
                            member.Member as IProperty, 
                            baseObjectIdentifierFunc(member), 
                            baseObjectMemberNameFunc(member)));
                }
                #endregion

                #region Process Fields
                else if (member.Member is IField)
                {
                    CodeGeneratorProxy.CreateProperty(
                        generateMemberModifier(member),
                        generateReturnTypeFunc(member),
                        generateMemberNameFunc(member),
                        GetFieldGetterStatement(
                            member.Member as IField,
                            baseObjectIdentifierFunc(member),
                            baseObjectMemberNameFunc(member)),
                        GetFieldSetterStatement(
                            member.Member as IField,
                            baseObjectIdentifierFunc(member),
                            baseObjectMemberNameFunc(member)));
                }
                #endregion
            }

        }
    }
}
