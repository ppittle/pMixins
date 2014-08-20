//----------------------------------------------------------------------- 
// <copyright file="ResolveAttributes.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps
{
    public class ResolveAttributes : IPipelineStep<IResolveAttributesPipelineState>
    {
        private readonly IEnumerable<IPipelineStep<IResolveAttributesPipelineState>> _attributePipelines =
            new List<IPipelineStep<IResolveAttributesPipelineState>>
                {
                   new ResolvePMixinAttributes(),
                   new ResolveInterceptorMixinRequirements()
                };

        public bool PerformTask(IResolveAttributesPipelineState pipelineState)
        {
            return (_attributePipelines.RunPipeline(pipelineState,
                                                    haltOnStepFailing: null,
                                                    throwException: null));
        }
    }
}
