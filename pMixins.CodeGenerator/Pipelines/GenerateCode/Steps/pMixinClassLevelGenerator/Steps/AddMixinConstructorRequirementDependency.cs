//----------------------------------------------------------------------- 
// <copyright file="AddMixinConstructorRequirementDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 28, 2014 3:02:57 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using CopaceticSoftware.pMixins.Infrastructure;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.pMixinClassLevelGenerator.Steps
{
    /// <summary>
    /// Adds the <see cref="IMixinConstructorRequirement{TMixin}"/> dependency
    /// </summary>
    public class AddMixinConstructorRequirementDependency : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public static string GetMixinConstructorRequirement(pMixinGeneratorPipelineState manager)
        {
            var requiredMixinWrapperType =
                (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract)
                    ? ExternalGeneratedNamespaceHelper.GenerateChildClassFullName(
                        manager,
                        GenerateAbstractMixinMembersWrapperClass.GetWrapperClassName(manager))
                    : manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal();

            return string.Format("global::{0}<{1}>",
                typeof (IMixinConstructorRequirement<>).GetOriginalFullName().Replace("<>",""),
                requiredMixinWrapperType);
        }

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            //Current strategy is to only add the requirement if explicitly
            //stated.  This follows the MVC ControllerFactory model, where
            //DI can be used to create instances of the mixin.
            if (!manager.CurrentpMixinAttribute.ExplicitlyInitializeMixin)
                return true;

            /*
            bool hasANonParameterlessConstructor =
                manager.CurrentpMixinAttribute.Mixin.GetConstructors()
                    .Any(c => !c.Parameters.Any());

            if (hasANonParameterlessConstructor &&
                !manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract)
                return true;
             */

            manager.GeneratedClassInterfaceList.Add
                (GetMixinConstructorRequirement(manager));

            return true;
        }
    }
}
