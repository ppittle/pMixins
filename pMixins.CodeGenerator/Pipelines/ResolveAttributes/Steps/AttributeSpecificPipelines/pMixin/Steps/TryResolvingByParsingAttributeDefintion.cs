//----------------------------------------------------------------------- 
// <copyright file="TryResolvingByParsingAttributeDefintion.cs" company="Copacetic Software"> 
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
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps
{
    public class TryResolvingByParsingAttributeDefintion :  IPipelineStep<ResolvepMixinAttributePipelineState>
    {
        public bool PerformTask(ResolvepMixinAttributePipelineState manager)
        {
            if (!manager.ResolvedResult.Mixin.IsNullOrUnkown())
                //ResolvedResult has already been resolved!
                return true;

            var attribute = manager.ResolvedResult.AttributeDefinition;

            manager.ResolvedResult.Mixin =
                attribute.GetNamedArgumentValue("Mixin") as IType;

            var masks = attribute.GetNamedArgumentValue("Masks") as IEnumerable<object>;

            if (null != masks)
                manager.ResolvedResult.Masks = masks.OfType<IType>().ToList();

            var interceptors = attribute.GetNamedArgumentValue("Interceptors") as IEnumerable<object>;

            if (null != interceptors)
                manager.ResolvedResult.Interceptors = interceptors.OfType<IType>().ToList();
                    
            var generateExtensionsMethodWrappers = 
                attribute.GetNamedArgumentValue("GenerateExtensionMethodWrappers");

            if (null != generateExtensionsMethodWrappers)
                manager.ResolvedResult.GenerateExtensionMethodWrappers =
                    bool.Parse(generateExtensionsMethodWrappers.ToString());

            var explicitlyInitializeMixin =
               attribute.GetNamedArgumentValue("ExplicitlyInitializeMixin");

            if (null != explicitlyInitializeMixin)
                manager.ResolvedResult.ExplicitlyInitializeMixin =
                    bool.Parse(explicitlyInitializeMixin.ToString());

            var enableSharedRequirementsInterface =
               attribute.GetNamedArgumentValue("EnableSharedRequirementsInterface");

            if (null != explicitlyInitializeMixin)
                manager.ResolvedResult.EnableSharedRequirementsInterface =
                    bool.Parse(enableSharedRequirementsInterface.ToString());

            var loggingVerbosity = attribute.GetNamedArgumentValue("LoggingVerbosity");

            if (null != loggingVerbosity)
                manager.ResolvedResult.LoggingVerbosity = (LoggingVerbosity)
                                                                   Enum.Parse(typeof(LoggingVerbosity),
                                                                   loggingVerbosity.ToString());
            return true;
                
        }
    }
}
