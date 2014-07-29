//----------------------------------------------------------------------- 
// <copyright file="AddIContainMixinInterfacesToTargetCodeBehind.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 29, 2014 7:12:59 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGeneratorProxy;
using CopaceticSoftware.pMixins.ConversionOperators;
using Mono.Cecil.Cil;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps.GenerateMembers
{
    /// <summary>
    /// Adds an <see cref="IContainMixin{TMixin}"/> implementation for every
    /// <see cref="MixinGenerationPlan"/> to 
    /// <see cref="TargetLevelCodeGeneratorPipelineState.TargetCodeBehindTypeDeclaration"/>:
    /// <code>
    /// <![CDATA[
    /// public partial class Target :  CopaceticSoftware.pMixins.ConversionOperators.IContainMixin<ExampleMixin>
    /// {
    ///     CopaceticSoftware.pMixins.ConversionOperators.IContainMixin<ExampleMixin>.MixinInstance
    ///     {
	///		    get
	///         {
	///			    return ___mixins.Test_MixinWithAttributes._mixinInstance;
	///		    }
	///	    }
    /// }
    /// ]]></code>
    /// </summary>
    public class AddIContainMixinInterfacesToTargetCodeBehind : IPipelineStep<TargetLevelCodeGeneratorPipelineState>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipelineState manager)
        {
            var codeGeneratorProxy =
                new CodeGeneratorProxy(manager.TargetCodeBehindTypeDeclaration);

            manager.CodeGenerationPlan.MixinGenerationPlans.Values
                .Where(mgp => mgp.AddAnIContainsMixinImplementation)
                .Map(mgp => ImplementIContainsMixin(codeGeneratorProxy, mgp));

            return true;
        }

        private void ImplementIContainsMixin(
            CodeGeneratorProxy codeBehind, 
            MixinGenerationPlan mgp)
        {
            var containMixinInterfaceName =
                string.Format("global::{0}.{1}<{2}>",
                              typeof(IContainMixin<>).Namespace,
                              "IContainMixin",
                              mgp.MixinAttribute.Mixin.GetOriginalFullNameWithGlobal());

            codeBehind.ImplementInterface(
                containMixinInterfaceName);

            codeBehind.CreateProperty(
                modifier:
                    //implement explicitly
                    string.Empty,
                returnTypeFullName:
                    mgp.MixinAttribute.Mixin.GetOriginalFullNameWithGlobal(),
                propertyName:
                    containMixinInterfaceName + ".MixinInstance",
                getterMethodBody:
                    string.Format(
                        "get{{ return {0}.{1}; }}",
                        mgp.MasterWrapperPlan.MasterWrapperInstanceNameAvailableFromTargetCodeBehind,
                        MasterWrapperPlan.MixinInstanceDataMemberName),
                setterMethodBody:
                    //no setter   
                    string.Empty
                );
        }
    }
}
