//----------------------------------------------------------------------- 
// <copyright file="GenerateProtectedWrapperMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 24, 2014 4:52:15 PM</date> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.MixinLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds members to the Protected Wrapper:
    /// <code>
    /// <![CDATA[
    /// public class MixinProtectedWrapper : Mixin
    /// {
    ///     //Wrap Constructors
    ///     public MixinProtectedWrappter(obj p1) : base (p1){}
    /// 
    ///     //Wrap methods
    ///     public new string ProtectedMethod()
    ///     {
    ///          return base.ProtectedMethod();
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </summary>
    public class GenerateProtectedWrapperMembers : IPipelineStep<MixinLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(MixinLevelCodeGeneratorPipelineState manager)
        {
            if (!manager.MixinGenerationPlan.ProtectedWrapperPlan.GenrateProtectedWrapper)
                return true;

            var codeGenerator =
                new CodeGeneratorProxy(manager.ProtectedMembersWrapper, "");

            ProcessConstructors(codeGenerator, manager);

            ProcessMembers(codeGenerator, manager);

            return true;
        }

        /// <summary>
        /// Creates simple public constructors that pass all parameters to base.
        /// <code>
        /// <![CDATA[
        /// public ProtectedWrapper(obj p1, obj p2) : base (p1, p2){}
        /// ]]></code>
        /// </summary>
        /// <param name="codeGenerator"></param>
        /// <param name="manager"></param>
        private void ProcessConstructors(CodeGeneratorProxy codeGenerator, MixinLevelCodeGeneratorPipelineState manager)
        {
            manager.MixinGenerationPlan.ProtectedWrapperPlan.Constructors
                .Map(c =>
                
                    codeGenerator.CreateConstructor(
                        modifiers:
                            "public",
                        parameters:
                            c.Parameters.ToKeyValuePair(),
                        constructorInitializer:
                            ": base(" + string.Join(",", c.Parameters.Select(p => p.Name)) + ")",
                        constructorBody:
                            string.Empty
                        )
                );
        }

        private void ProcessMembers(CodeGeneratorProxy codeGenerator, MixinLevelCodeGeneratorPipelineState manager)
        {
            var proxyMemberHelper =
                new CodeGeneratorProxyMemberHelper(
                    codeGenerator,
                    manager.CommonState.Context.TypeResolver.Compilation);

            proxyMemberHelper.CreateMembers(
                members:
                    manager.MixinGenerationPlan.ProtectedWrapperPlan.Members,
                generateMemberModifier:
                    m => "public new " + (m.Member.IsStatic ? " static" : ""),
                baseObjectIdentifierFunc:
                    m => 
                        (m.Member.IsStatic)
                        ? m.MixinAttribute.Mixin.GetOriginalFullNameWithGlobal()
                        : "base");
        }
    }
}
