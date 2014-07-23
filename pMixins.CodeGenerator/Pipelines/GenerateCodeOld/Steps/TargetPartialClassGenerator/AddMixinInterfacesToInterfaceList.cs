//----------------------------------------------------------------------- 
// <copyright file="AddMixinInterfacesToInterfaceList.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 7, 2014 5:32:09 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.TargetPartialClassGenerator
{
    public class AddMixinInterfacesToInterfaceList : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var doNotMixinType = typeof (DoNotMixinAttribute)
                .ToIType(manager.BaseState.Context.TypeResolver.Compilation);

            var currentMixin = manager.CurrentpMixinAttribute.Mixin;

            var candidateTypes =
                //If Masks are defined, only use the masks
                manager.CurrentpMixinAttribute.Masks.Any()
                    ? manager.CurrentpMixinAttribute.Masks
                        .Union(manager.CurrentpMixinAttribute.Masks.SelectMany(x => x.GetAllBaseTypes()))
                    //otherwise use all base types from mixin
                    : currentMixin
                        .GetDefinition().GetAllBaseTypes();

            manager.GeneratedClassInterfaceList.AddRange(
                candidateTypes
                    .Where(x => x.Kind == TypeKind.Interface &&
                                !x.IsDecoratedWithAttribute(doNotMixinType, includeBaseTypes: false))
                    .Select(x => x.GetOriginalFullNameWithGlobal(currentMixin)));
            
            return true;
        }
    }
}
