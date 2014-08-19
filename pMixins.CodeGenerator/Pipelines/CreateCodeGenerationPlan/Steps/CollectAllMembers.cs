//----------------------------------------------------------------------- 
// <copyright file="CollectAllMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 10:15:58 AM</date> 
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
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Extensions;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps
{
    public class CollectAllMembers : IPipelineStep<ICreateCodeGenerationPlanPipelineState>
    {
        public bool PerformTask(ICreateCodeGenerationPlanPipelineState manager)
        {
            var memberCollector = new MemberCollector();

            foreach (var target in manager.CommonState.SourcePartialClassDefinitions)
            {
                foreach (var mixin in manager.GetAllPMixinAttributes(target))
                {
                    
                    var mixinMembers =
                        memberCollector.CollectMemberWrappers(
                            mixin, 
                            manager.CommonState.Context.TypeResolver.Compilation)
                        .ToList();

                    //Save Members
                    manager.CodeGenerationPlans[target].MixinGenerationPlans[mixin].Members = mixinMembers;
                }
            }

            return true;
        }

        /// <summary>
        /// Placed in a child class so it can be unit tested.
        /// </summary>
        public class MemberCollector
        {
            public IEnumerable<MemberWrapper> CollectMemberWrappers(
                pMixinAttributeResolvedResult mixinAttribute,
                ICompilation compilation)
            {
                return CollectMemberWrappers(
                    mixinAttribute,
                    BuildMemberFilterFunction(mixinAttribute, compilation));
            }

            /// <summary>
            /// There be dragons here!
            /// </summary>
            public IEnumerable<MemberWrapper> CollectMemberWrappers(
                pMixinAttributeResolvedResult mixinAttribute,
                Func<IMember, bool> memberFilter)
            {
                
                var declaredMembers =
                    mixinAttribute.Mixin
                        .GetMembers()
                        .Where(memberFilter)
                        //Order by inheritance depth, so 'DistinctMembers' will 
                        //remove the member furthest from mixinAttribute.Mixin
                        .OrderByDescending(x => x.DeclaringType.InheritanceDepth())
                        //Remove inherited duplicates
                        .DistinctMembers()
                        .Select(m =>
                            new MemberWrapper
                            {
                                DeclaringType = mixinAttribute.Mixin,

                                Member = m,

                                MixinAttribute = mixinAttribute
                            })
                        .ToList();

                //collect shadowed interface members
                var interfaceMembers =
                    mixinAttribute.Mixin
                        .GetAllBaseTypes()
                        .Where(t => t.GetDefinition().Kind == TypeKind.Interface)
                        .SelectMany(t =>
                            t.GetMembers()
                                .Where(memberFilter)
                                .Select(m =>
                                    new MemberWrapper(
                                        new MemberImplementationDetails
                                        {
                                            ExplicitInterfaceImplementationType = t
                                        })
                                    {
                                        DeclaringType = t,

                                        Member = m,

                                        MixinAttribute = mixinAttribute
                                    }))
                        //Remove inherited duplicates
                        .DistinctMemberWrappers(includeDeclaringTypeInComparison: true)
                        //Remove any members that were already declared
                        //explicitly implemented in the declaredMembers.
                        .FilterMemberWrappers(
                            declaredMembers.Where(x => x.Member.IsExplicitInterfaceImplementation));

                var allMembers =
                    declaredMembers
                        .Union(interfaceMembers)
                        .ToList();
              

                //Handle Mixin Masks
                if (mixinAttribute.Masks.IsNullOrEmpty())
                    return allMembers;

                var maskMethods =
                    mixinAttribute.Masks
                        .SelectMany(x => x.GetMembers())
                        .Where(memberFilter);

                return
                    allMembers
                        .Where(mw => maskMethods.Any(mm => mm.EqualsMember(mw.Member)));
            }

            public Func<IMember, bool> BuildMemberFilterFunction(
                pMixinAttributeResolvedResult mixinAttribute,
                ICompilation compilation)
            {
                var includeInternalMembers =
                    mixinAttribute.Mixin.GetDefinition().ParentAssembly.Equals(
                        compilation.MainAssembly);

                var doNotMixinIType =
                    typeof (DoNotMixinAttribute)
                        .ToIType(compilation);

                return new Func<IMember, bool>(
                    member => (
                        !member.IsPrivate &&
                        (!member.IsProtected || !mixinAttribute.Mixin.GetDefinition().IsSealed) &&
                        (!member.IsInternal || includeInternalMembers) &&
                        !member.FullName.StartsWith("System.Object") &&
                        !member.IsDecoratedWithAttribute(doNotMixinIType) &&
                        !member.DeclaringType.IsDecoratedWithAttribute(doNotMixinIType, includeBaseTypes: false)));


            }
        }
    }
}
