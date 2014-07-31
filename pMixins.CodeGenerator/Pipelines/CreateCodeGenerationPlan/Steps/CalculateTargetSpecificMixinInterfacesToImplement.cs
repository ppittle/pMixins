//----------------------------------------------------------------------- 
// <copyright file="CalculateTargetSpecificMixinInterfacesToImplement.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 29, 2014 11:29:54 AM</date> 
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
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using CopaceticSoftware.pMixins.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    /// <summary>
    /// Populates <see cref="TargetCodeBehindPlan.MixinInterfaces"/>
    /// </summary>
    public class CalculateTargetSpecificMixinInterfacesToImplement : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            var doNotMixinIType =
                typeof (DoNotMixinAttribute).ToIType(
                    manager.CommonState.Context.TypeResolver.Compilation);

            foreach (var cgp in manager.CodeGenerationPlans.Values)
            {
                cgp.TargetCodeBehindPlan.MixinInterfaces =
                    cgp.MixinGenerationPlans.Values
                        .SelectMany(mgp =>
                        
                            //If masks have been defined, only use the masks,
                            //otherwise, use all Mixin base types.
                            mgp.MixinAttribute.Masks.IsNullOrEmpty()
                                ?  mgp.MixinAttribute.Mixin.GetAllBaseTypes()
                                :  mgp.MixinAttribute.Masks
                        )
                        .Where(bt => bt.Kind == TypeKind.Interface)
                        //Filter out interfaces decorated with DoNotMixin
                        .Where(bt => !bt.IsDecoratedWithAttribute(doNotMixinIType))
                        .Distinct()
                        .ToList();

                if (!EnsureMixinDependenciesAreSatisfied(manager, cgp))
                    return false;
            }

            return true;
        }

        private static readonly string _mixinDependencyTypeName = typeof(IMixinDependency<>).GetOriginalFullName()
            //This is hacky, but can't find a way to compare typeof(IMixinDependency<>) to new IType(IMixinDependency<int>)
            .Replace("<>", "");

        public static bool TypeIsIMixinDependency(IType type)
        {
            return type.Kind == TypeKind.Interface &&
                   type.GetOriginalFullName().StartsWith(_mixinDependencyTypeName);
        }

        private bool EnsureMixinDependenciesAreSatisfied(
            ICreateCodeGenerationPlanPipelineState manager,
            CodeGenerationPlan cgp)
        {
            var mixinBaseTypeMap =
                cgp.MixinGenerationPlans.Values
                    .Select(mgp => mgp.MixinAttribute)
                    .ToDictionary(
                        x => x.Mixin.GetOriginalFullName(),
                        x => x.Mixin.GetAllBaseTypes());

            var allBaseTypes = 
                mixinBaseTypeMap
                    .SelectMany(x => x.Value)
                    .Union(
                        cgp.SourceClass.BaseTypes
                            .Select(bt => manager.CommonState.Context.TypeResolver.Resolve(bt).Type))
                    .ToList();

          var unsatisfiedDependencies =
                mixinBaseTypeMap
                    //Take only Mixin base types
                    .SelectMany(x => x.Value)
                    //Get IMixinDependency
                    .Where(bt => TypeIsIMixinDependency(bt))
                    //Cast in order to get generic parameter
                    .OfType<ParameterizedType>()
                    //Pull out generic param
                    .Select(bt => bt.TypeArguments.First())
                    //Filter out 
                    .Where(md => !allBaseTypes.Contains(md))
                    .ToList();

            //Add interface dependencies to Target
            cgp.TargetCodeBehindPlan.MixinInterfaces.AddRange(
                unsatisfiedDependencies
                    .Where(md => md.Kind == TypeKind.Interface));

            var unsatisfiedClassDependencies =
                unsatisfiedDependencies
                    .Where(md => md.Kind != TypeKind.Interface)
                    .ToList();

            //Write out errors
            if (unsatisfiedClassDependencies.Count > 0)
            {
                foreach (var dep in unsatisfiedDependencies)
                {
                    var mixin =
                       cgp.MixinGenerationPlans.Values.Select(mgp => mgp.MixinAttribute)
                            .First(mix =>
                                mix.Mixin.FullName ==
                                mixinBaseTypeMap
                                    .First(x =>
                                        x.Value.OfType<ParameterizedType>()
                                            .Any(p => p.TypeArguments.First().Equals(dep)))
                                    .Key);

            
                    manager.CommonState.CodeGenerationErrors.Add(
                        new CodeGenerationError
                        {
                            
                            Message = string.Format(
                                Strings.ErrorMixinDependencyIsClassAndIsNotSatisified,
                                mixin.Mixin.GetOriginalFullName(),
                                cgp.SourceClass.Name,
                                dep.GetOriginalFullName()),


                            Line = (uint)cgp.SourceClass.GetRegion().BeginLine,
                            Column = (uint)cgp.SourceClass.GetRegion().EndLine,
                            Severity = CodeGenerationError.SeverityOptions.Error
                        });
                }

                return false;
            }

            return true;
        }
    }
}
