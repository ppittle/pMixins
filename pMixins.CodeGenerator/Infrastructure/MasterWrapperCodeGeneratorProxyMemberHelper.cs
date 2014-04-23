//----------------------------------------------------------------------- 
// <copyright file="MasterWrapperCodeGeneratorProxyMemberHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, February 26, 2014 12:33:00 AM</date> 
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
using System.Security.Permissions;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.pMixins.Interceptors;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure
{
    //TODO: Add support for static members
    public class MasterWrapperCodeGeneratorProxyMemberHelper : CodeGeneratorProxyMemberHelper
    {
        public MasterWrapperCodeGeneratorProxyMemberHelper(ICodeGeneratorProxy codeGeneratorProxy,
            ICompilation compilation)
            : base(codeGeneratorProxy, compilation)
        {
        }

        protected override string GetMethodBodyStatementImpl(
            IMethod method,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> methodNameFunc)
        {
            if (method.IsStatic)
                return base.GetMethodBodyStatementImpl(method, baseObjectIdentifierFunc, methodNameFunc);

            return string.Format("{0} {1};",
                method.GetReturnString(),
                GetMethodBodyCallStatement(method, baseObjectIdentifierFunc, methodNameFunc));
        }

        public string GetMethodBodyCallStatement(
            IMethod method, 
            Func<IMember,string> baseObjectIdentifierFunc, 
            Func<IMember,string> methodNameFunc)
        {
            if (method.IsStatic)
                return base.GetMethodBodyStatementImpl(method, baseObjectIdentifierFunc, methodNameFunc);

            var masterWrapperBaseMethod =
                string.IsNullOrEmpty(method.GetReturnString())
                    ? "ExecuteVoidMethod"
                    : "ExecuteMethod";

            return string.Format(
                @"base.{0}(
                            ""{1}"",
                            new global::System.Collections.Generic.List<{2}>{{{3}}},
                            () => {4}.{5}({6}))",
                
                masterWrapperBaseMethod,
                method.GetOriginalName(),
                "global::" + typeof(Parameter).FullName,

                string.Join(",",
                    method.Parameters.Select(p => 
                        string.Format(
                            @"new {0}{{
                                                Name=""{1}"",
                                                Type = typeof({2}),
                                                Value = {1}
                                            }}",
                            "global::" + typeof(Parameter).FullName,
                            p.Name,
                            TypeExtensions.GetOriginalFullNameWithGlobal(p.Type)
                            ))),

                baseObjectIdentifierFunc(method),
                methodNameFunc(method),
                string.Join(",", method.Parameters.Select(x => x.Name)));
        }

        protected override string GetPropertyGetterStatementImpl(
            IProperty prop, 
            Func<IMember, string> baseObjectIdentifierFunc, 
            Func<IMember, string> propertyNameFunc)
        {
            if (prop.IsStatic)
                return base.GetPropertyGetterStatementImpl(prop, baseObjectIdentifierFunc, propertyNameFunc);


            return string.Format("get{{ return {0}; }}",
                GetPropertyGetterReturnBodyStatement(prop, baseObjectIdentifierFunc, propertyNameFunc));
        }

        public string GetPropertyGetterReturnBodyStatement(
            IProperty prop,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> propertyNameFunc)
        {
            return string.Format("base.ExecutePropertyGet(\"{0}\", () => {1}.{2})",
                prop.Name,
                baseObjectIdentifierFunc(prop),
                propertyNameFunc(prop));
        }

        protected override string GetPropertySetterStatementImpl(
            IProperty prop, 
            Func<IMember, string> baseObjectIdentifierFunc, 
            Func<IMember, string> propertyNameFunc)
        {
            if (prop.IsStatic)
                return base.GetPropertySetterStatementImpl(prop, baseObjectIdentifierFunc, propertyNameFunc);

            return string.Format("set{{ {0}; }}",
               GetPropertySetterReturnBodyStatement(prop, baseObjectIdentifierFunc, propertyNameFunc));
        }

        public string GetPropertySetterReturnBodyStatement(
            IProperty prop,
            Func<IMember, string> baseObjectIdentifierFunc,
            Func<IMember, string> propertyNameFunc)
        {
            return string.Format("base.ExecutePropertySet(\"{0}\", value, (v) => {1}.{2} = v)",
                prop.Name,
                baseObjectIdentifierFunc(prop),
                propertyNameFunc(prop));
        }
    }
}
