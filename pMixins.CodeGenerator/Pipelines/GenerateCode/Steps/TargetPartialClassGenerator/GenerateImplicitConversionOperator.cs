//----------------------------------------------------------------------- 
// <copyright file="GenerateImplicitConversionOperator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, January 28, 2014 12:17:05 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.TargetPartialClassGenerator
{
    /// <summary>
    /// Create the Static Implicit Operator like
    /// <code><![CDATA[
    /// public static implicit operator ExampleMixin(BasicConceptSpec spec)
    /// {
    ///     return spec.__mixins._ExampleMixin.Value;
    /// }
    /// ]]></code>
    /// </summary>
    public class GenerateImplicitConversionOperator : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            if (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsStatic)
                return true;

            var operatorTypeName =
                GetBestTypeCandidate(manager.CurrentpMixinAttribute.Mixin);

            if (string.IsNullOrEmpty(operatorTypeName))
                return true;
            
            manager.GeneratedClass.CreateMethod(
                "public static implicit",
                "operator",
                operatorTypeName,
                new List<KeyValuePair<string, string>>{new KeyValuePair<string, string>(manager.GeneratedClass.ClassName, "target")},
                string.Format("return target.{0}.{1};",
                    manager.CurrentMixinInstanceVariableAccessor,
                    GenerateMixinMasterWrapperClass.MixinInstanceDataMemberName)
                );

            return true;
        }

        private string GetBestTypeCandidate(IType type)
        {
            if (type.GetDefinition().IsPublic)
                return type.GetOriginalFullNameWithGlobal();

            return type.GetDefinition().GetAllBaseTypes()
                .Where(t => t.GetDefinition().IsPublic
                                     && t.GetDefinition().FullName.ToLower() != "system.object")
                .Select(t => t.GetOriginalFullNameWithGlobal())
                .FirstOrDefault();
        }
    }
}
