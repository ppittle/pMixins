//----------------------------------------------------------------------- 
// <copyright file="ResolvepMixinAttributes.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 12:33:09 AM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin
{
    public class ResolvepMixinAttributes : IPipelineStep<IResolveAttributesPipelineState>
    {
        private readonly IEnumerable<IPipelineStep<ResolvepMixinAttributePipelineState>> _pipeline =
            new List<IPipelineStep<ResolvepMixinAttributePipelineState>>
                {
                    new EnsureAttributeTargetIsPartialClass(),
                    new TryResolvingByCreatingpMixinInstance(),
                    new TryResolvingByParsingAttributeDefintion(),
                    new PrivateNestedTypeFix(),
                    new ValidateMixinType(),
                    new ValidateMaskTypes(),
                    new ValidateInterceptorTypes()
                };

        public bool PerformTask(IResolveAttributesPipelineState manager)
        {
            foreach (var classAtts in manager.SourcePartialClassAttributes)
            {
                foreach (var att in classAtts.Value.Where(x =>
                    x.AttributeType.Implements<pMixinAttribute>()))
                {
                    var pipelineState = new ResolvepMixinAttributePipelineState
                    {
                        BaseState = manager,

                        TargetClassDefintion = classAtts.Key,
                        
                        ResolvedResult = new pMixinAttributeResolvedResult(att)
                    };

                    if (!_pipeline.RunPipeline(pipelineState,
                            haltOnStepFailing: null,
                            throwException: null))
                        return false;

                    manager.PartialClassLevelResolvedpMixinAttributes[classAtts.Key]
                        .Add(pipelineState.ResolvedResult);
                }
            }

            return true;
        }
    }
}
