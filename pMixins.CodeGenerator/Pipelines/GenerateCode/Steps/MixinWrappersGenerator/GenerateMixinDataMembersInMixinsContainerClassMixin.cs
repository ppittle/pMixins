//----------------------------------------------------------------------- 
// <copyright file="GenerateMixinDataMembersInMixinsContainerClassMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, January 28, 2014 2:13:07 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PostClassGeneration;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator
{
    /// <summary>
    /// Creates the DataMember inside the Mixins container class and sets
    /// <see cref="pMixinGeneratorPipelineState.CurrentMixinInstanceVariableAccessor"/> so other steps
    /// can access the Mixin instance.  Additionally, adds a <see cref="pMixinGeneratorPipelineState.MixinContainerClassConstructorStatements"/>
    /// for initializing the DataMember.
    /// Code like:
    /// <code><![CDATA[
    /// private sealed class __Mixins //__Mixins class created in GenerateMixinsContainerClass
    /// {
    ///     public __Mixins(HostType host)
    ///     {
    ///         _ExampleMixin = new DefaultMixinActivator().CreateInstance<ExampleMixin>());
    ///     }
    /// 
    ///     public readonly Lazy<ExampleMixin> _ExampleMixin;
    /// }
    /// ]]></code>
    /// </summary>
    public class GenerateMixinDataMembersInMixinsContainerClassMixin : IPipelineStep<pMixinGeneratorPipelineState>
    {
       public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            if (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsStatic)
            {
                //Static mixed in classes can be accessed directly, they don't need an instance
                manager.CurrentMixinInstanceVariableAccessor =
                    manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal();

                return true;
            }

            if (null == manager.MixinContainerClassGeneratorProxy)
                throw new Exception(" manager.MixinContainerClassGeneratorProxy is null.  Did the GenerateMixinsContainerClass step run?");

            var currentMixinInstanceVariable =
                manager.CurrentpMixinAttribute.Mixin.GetFullNameAsIdentifier();

            manager.CurrentMixinInstanceVariableAccessor =
                GenerateMixinsContainerClass.MixinContainerPropertyName + "." +
                currentMixinInstanceVariable;

            manager.MixinContainerClassGeneratorProxy.CreateDataMember
                ("public readonly",
                 manager.CurrentMixinMasterWrapperFullTypeName,
                 currentMixinInstanceVariable);


            var dataMemberInitializationConstructorParamsString = 
                    string.IsNullOrEmpty(manager.CurrentMixinRequirementsInterface)
                    ? string.Empty
                    : string.Format("({0}){1}",
                            manager.CurrentMixinRequirementsInterface.EnsureStartsWith("global::"),
                            GenerateMixinsContainerClassConstructor.MixinContainerConstructorParameterName);

            manager.MixinContainerClassConstructorStatements.Add(
                currentMixinInstanceVariable + " = " +
                    Extensions.TypeExtensions.GenerateActivationExpression(
                        manager.CurrentMixinMasterWrapperFullTypeName,
                         dataMemberInitializationConstructorParamsString));
            
            return true;
        }
    }
}
