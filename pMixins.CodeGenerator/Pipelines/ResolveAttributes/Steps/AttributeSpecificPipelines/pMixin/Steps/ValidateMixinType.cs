//----------------------------------------------------------------------- 
// <copyright file="ValidateMixinType.cs" company="Copacetic Software"> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps
{
    public class ValidateMixinType : IPipelineStep<ResolvePMixinAttributePipelineState>
    {
        public bool PerformTask(ResolvePMixinAttributePipelineState manager)
        {
            var pMixinResolvedResult = manager.ResolvedResult;
            
            if (null == pMixinResolvedResult.Mixin)
                #region Log Error and Return False
                {
                    manager.BaseState.CodeGenerationErrors.Add(
                        new CodeGenerationError(
                            Strings.ErrorMixinTypeIsNotSpecified,
                            CodeGenerationError.SeverityOptions.Error,
                            manager.TargetClassDefinition.StartLocation.Line,
                            manager.TargetClassDefinition.StartLocation.Column));

                    return false;
                }
                #endregion

            if (pMixinResolvedResult.Mixin.Kind == TypeKind.Unknown)
                #region Log Error and Return False
                {
                    manager.BaseState.CodeGenerationErrors.Add(
                        new CodeGenerationError(
                            string.Format(
                                Strings.ErrorMixinCouldNotBeResolved,
                                pMixinResolvedResult.Mixin.Name),
                            CodeGenerationError.SeverityOptions.Error,
                            manager.TargetClassDefinition.StartLocation.Line,
                            manager.TargetClassDefinition.StartLocation.Column));

                    return false;
                }
                #endregion

            if (pMixinResolvedResult.Mixin.Kind == TypeKind.Interface ||
                pMixinResolvedResult.Mixin.Kind == TypeKind.Struct)
                #region Log Error and Return False
                {
                    manager.BaseState.CodeGenerationErrors.Add(
                        new CodeGenerationError(
                            string.Format(
                                Strings.ErrorMixinCanNotBeInterfaceOrStruct,
                                pMixinResolvedResult.Mixin.GetOriginalFullName()),
                            CodeGenerationError.SeverityOptions.Error,
                            manager.TargetClassDefinition.StartLocation.Line,
                            manager.TargetClassDefinition.StartLocation.Column));

                    return false;
                }
                #endregion

            if (pMixinResolvedResult.Mixin.GetDefinition().IsAbstract &&
                pMixinResolvedResult.Mixin.GetDefinition().IsNestedType())
                #region Log Error and Return False
                {
                    manager.BaseState.CodeGenerationErrors.Add(
                        new CodeGenerationError(
                            string.Format(
                                Strings.ErrorMixinIsNestedAndAbstract,
                                pMixinResolvedResult.Mixin.GetOriginalFullName()),
                            CodeGenerationError.SeverityOptions.Error,
                            manager.TargetClassDefinition.StartLocation.Line,
                            manager.TargetClassDefinition.StartLocation.Column));

                    return false;
                }
                #endregion

            foreach (var mixinMember in pMixinResolvedResult.Mixin.GetMembers())
            {
                if (mixinMember.ReturnType.IsUnkown())
                    #region Log Error and Return False
                    {
                        manager.BaseState.CodeGenerationErrors.Add(
                            new CodeGenerationError(
                                string.Format(
                                    Strings.ErrorTypeInMixinMemberCouldNotBeResolved,
                                    pMixinResolvedResult.Mixin.GetOriginalFullName(),
                                    "Return Type",
                                    mixinMember.ReturnType.GetOriginalFullName()),
                                CodeGenerationError.SeverityOptions.Error,
                                manager.TargetClassDefinition.StartLocation.Line,
                                manager.TargetClassDefinition.StartLocation.Column));

                        return false;
                    }
                    #endregion

                if (!(mixinMember is IMethod))
                    continue;

                foreach (var memberParamType in (mixinMember as IMethod).Parameters
                    .Where(memberParamType => memberParamType.Type.IsUnkown()))
                        #region Log Error and Return False
                        {
                            manager.BaseState.CodeGenerationErrors.Add(
                                new CodeGenerationError(
                                    string.Format(
                                        Strings.ErrorTypeInMixinMemberCouldNotBeResolved,
                                        pMixinResolvedResult.Mixin.GetOriginalFullName(),
                                        "Method Parameter Type",
                                        memberParamType.Type.GetOriginalFullName()),
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
