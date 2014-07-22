//----------------------------------------------------------------------- 
// <copyright file="EnsureMixinDependenciesAreSatisfiedOnTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 5:45:59 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using CopaceticSoftware.pMixins.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PostClassGeneration
{
    /// <summary>
    /// Responsible for enforcing <see cref="IMixinDependency{T}"/>s are satisfied.  If
    /// Target does not implement / inherit the required dependency, or it is not
    /// mixed in, then if the dependency is an interface, it is added to Target.  If 
    /// the dependency is a class, an error is added. 
    /// </summary>
    /// <remarks>
    /// Missing interface dependencies are added to 
    /// <see cref="pMixinGeneratorPipelineState.GeneratedClassInterfaceList"/>
    /// to be added by <see cref="AddInterfacesToGeneratedContainerClass"/>.
    /// </remarks>
    public class EnsureMixinDependenciesAreSatisfiedOnGeneratedClass : IPipelineStep<pMixinGeneratorPipelineState>
    {
        private static readonly string _mixinDependencyTypeName = typeof (IMixinDependency<>).GetOriginalFullName()
            //This is hacky, but can't find a way to compare typeof(IMixinDependency<>) to new IType(IMixinDependency<int>)
            .Replace("<>", "");

        public static bool TypeIsIMixinDependency(IType type)
        {
            return type.Kind == TypeKind.Interface &&
                   type.GetOriginalFullName().StartsWith(_mixinDependencyTypeName);
        }

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var mixinBaseTypeMap =
                manager.BaseState.PartialClassLevelResolvedPMixinAttributes[manager.SourceClass]
                    .OfType<pMixinAttributeResolvedResult>()
                    .ToDictionary(
                        x => x.Mixin.GetOriginalFullName(),
                        x => x.Mixin.GetAllBaseTypes());

            var allBaseTypes = 
                mixinBaseTypeMap
                    .SelectMany(x => x.Value)
                    .Union(
                        manager.SourceClass.BaseTypes
                            .Select(bt => manager.BaseState.Context.TypeResolver.Resolve(bt).Type))
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
            manager.GeneratedClassInterfaceList.AddRange(
                unsatisfiedDependencies
                    .Where(md => md.Kind == TypeKind.Interface)
                    .Select(x => x.GetOriginalFullNameWithGlobal()));

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
                        manager.BaseState.PartialClassLevelResolvedPMixinAttributes[manager.SourceClass]
                            .OfType<pMixinAttributeResolvedResult>()
                            .First(mix =>
                                mix.Mixin.FullName ==
                                mixinBaseTypeMap
                                    .First(x =>
                                        x.Value.OfType<ParameterizedType>()
                                            .Any(p => p.TypeArguments.First().Equals(dep)))
                                    .Key);

            
                    manager.BaseState.CodeGenerationErrors.Add(
                        new CodeGenerationError
                        {
                            
                            Message = string.Format(
                                Strings.ErrorMixinDependencyIsClassAndIsNotSatisified,
                                mixin.Mixin.GetOriginalFullName(),
                                manager.GeneratedClass.ClassName,
                                dep.GetOriginalFullName()),
                            

                            Line = (uint)manager.SourceClass.GetRegion().BeginLine,
                            Column = (uint)manager.SourceClass.GetRegion().EndLine,
                            Severity = CodeGenerationError.SeverityOptions.Error
                        });
                }

                return false;
            }

            return true;
        }
    }
}
