//----------------------------------------------------------------------- 
// <copyright file="AddIContainMixinInterface.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 23, 2014 1:59:56 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;
using CopaceticSoftware.pMixins.ConversionOperators;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.TargetPartialClassGenerator
{
    public class AddIContainMixinInterface : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var containMixinInterfaceName =
                string.Format("global::{0}.{1}<{2}>",
                              typeof (IContainMixin<>).Namespace,
                              "IContainMixin",
                              manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal());

            manager.GeneratedClassInterfaceList.Add(
                containMixinInterfaceName);

            manager.GeneratedClass.CreateProperty(
                "",
                manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal(),
                containMixinInterfaceName + ".MixinInstance",
                string.Format(
                    "get{{ return {0}.{1}; }}",
                    manager.CurrentMixinInstanceVariableAccessor,
                    GenerateMixinMasterWrapperClass.MixinInstanceDataMemberName),
                "");


            return true;
        }
    }
}
