//----------------------------------------------------------------------- 
// <copyright file="ValidateMaskTypes.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Interceptors;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps
{
    public class ValidateInterceptorTypes : IPipelineStep<ResolvePMixinAttributePipelineState>
    {
        public bool PerformTask(ResolvePMixinAttributePipelineState manager)
        {
            foreach (var interceptor in manager.ResolvedResult.Interceptors)
            {
                if (interceptor.Kind != TypeKind.Class ||
                    interceptor.GetDefinition().IsAbstract)
                    #region Log Error and Return False
                    {
                        manager.BaseState.CommonState.CodeGenerationErrors.Add(
                            new CodeGenerationError(
                                string.Format(
                                    Strings.ErrorInterceptorMustBeConcreteClass,
                                    interceptor.Name),
                                CodeGenerationError.SeverityOptions.Error,
                                manager.TargetClassDefinition.StartLocation.Line,
                                manager.TargetClassDefinition.StartLocation.Column));

                        return false;
                    }
                    #endregion

                if (!interceptor.GetDefinition().Implements<IMixinInterceptor>())
                    #region Log Error and Return False
                    {
                        manager.BaseState.CommonState.CodeGenerationErrors.Add(
                            new CodeGenerationError(
                                string.Format(
                                    Strings.ErrorInterceptorMustImplementIMixinIntercetpor,
                                    interceptor.Name,
                                    typeof(IMixinInterceptor).FullName),
                                CodeGenerationError.SeverityOptions.Error,
                                manager.TargetClassDefinition.StartLocation.Line,
                                manager.TargetClassDefinition.StartLocation.Column));

                        return false;
                    }
                    #endregion

            }
            return true;
        }
    }
}
