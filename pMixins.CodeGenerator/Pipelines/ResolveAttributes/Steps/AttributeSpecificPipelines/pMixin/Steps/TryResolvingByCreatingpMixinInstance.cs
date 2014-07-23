//----------------------------------------------------------------------- 
// <copyright file="TryResolvingByCreatingpMixinInstance.cs" company="Copacetic Software"> 
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

using System;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps
{
    public class TryResolvingByCreatingpMixinInstance : IPipelineStep<ResolvePMixinAttributePipelineState>
    {
        public bool PerformTask(ResolvePMixinAttributePipelineState manager)
        {
            var compilation = manager.BaseState.Context.TypeResolver.Compilation;

            manager.BaseState.TypeInstanceActivator.TryCreateInstance<pMixinAttribute>(
                manager.ResolvedResult.AttributeDefinition,
                pMixinAttributeInstance =>
                {
                    try
                    {
                        manager.ResolvedResult.Mixin =
                            pMixinAttributeInstance.Mixin.ToIType(compilation);

                        manager.ResolvedResult.Masks =
                            pMixinAttributeInstance.Masks.Select(t => t.ToIType(compilation)).ToList();

                        manager.ResolvedResult.Interceptors =
                            pMixinAttributeInstance.Interceptors.Select(t => t.ToIType(compilation)).ToList();

                        manager.ResolvedResult.LoggingVerbosity =
                            pMixinAttributeInstance.LoggingVerbosity;

                        manager.ResolvedResult.GenerateExtensionMethodWrappers =
                            pMixinAttributeInstance.GenerateExtensionMethodWrappers;

                        manager.ResolvedResult.ExplicitlyInitializeMixin =
                            pMixinAttributeInstance.ExplicitlyInitializeMixin;

                        manager.ResolvedResult.EnableSharedRequirementsInterface =
                            pMixinAttributeInstance.EnableSharedRequirementsInterface;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("THIS IS A BUG: Unexpected exception when parsing an instance of pMixinAttribute", e);
                    }
                });

            return true;
        }
    }
}
