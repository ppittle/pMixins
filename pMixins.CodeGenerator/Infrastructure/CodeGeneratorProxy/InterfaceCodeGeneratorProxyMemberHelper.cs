//----------------------------------------------------------------------- 
// <copyright file="InterfaceCodeGeneratorProxyMemberHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 25, 2014 11:31:00 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy
{
    /// <summary>
    /// Wrapper for a <see cref="ICodeGeneratorProxy"/> that 
    /// specializes in creating <see cref="MemberWrapper"/>s
    /// for interfaces.
    /// </summary>
    public class InterfaceCodeGeneratorProxyMemberHelper 
    {
        protected readonly ICodeGeneratorProxy CodeGeneratorProxy;
        protected readonly ICompilation Compilation;

        public InterfaceCodeGeneratorProxyMemberHelper(ICodeGeneratorProxy codeGeneratorProxy, ICompilation compilation)
        {
            CodeGeneratorProxy = codeGeneratorProxy;
            Compilation = compilation;
        }


        public void CreateMembers(IEnumerable<MemberWrapper> memberWrappers)
        {
            foreach (var mw in memberWrappers)
            {
                #region Process Methods
                if (mw.Member is IMethod)
                {
                    CodeGeneratorProxy.CreateMethod(
                        modifier:
                            string.Empty, //no modifier for interface member
                        returnTypeFullName:
                            mw.Member.ReturnType.GetOriginalFullNameWithGlobal(),
                        methodName:
                            mw.ImplementationDetails.RequirementsInterfaceImplementationName,
                        parameters:
                            (mw.Member as IMethod).Parameters.ToKeyValuePair(),
                        methodBody:
                            string.Empty,
                        constraingClause:
                            (mw.Member as IMethod).GetGenericMethodConstraints(Compilation));
                }
                    #endregion

                    #region Process Properties
                else if (mw.Member is IProperty)
                {
                    CodeGeneratorProxy.CreateProperty(
                        modifier:
                            string.Empty, //no modifier for interface member
                        returnTypeFullName:
                            mw.Member.ReturnType.GetOriginalFullNameWithGlobal(),
                        propertyName:
                           mw.ImplementationDetails.RequirementsInterfaceImplementationName,
                        getterMethodBody:
                            (mw.Member as IProperty).CanGet ? "get;" : "",
                        setterMethodBody:
                            (mw.Member as IProperty).CanSet ? "set;" : "");
                }
                #endregion
            }
        }
    }
}
