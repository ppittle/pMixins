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
using System.Collections;
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
            foreach (var target in manager.CommonState.SourcePartialClassDefinitions)
            {
                foreach (var mixin in manager.GetAllPMixinAttributes(target))
                {
                    var memberFilter = BuildMemberFilterFunction(
                        mixin,
                        manager.CommonState.Context.TypeResolver.Compilation);

                    var mixinMembers = 
                        CollectMemberWrappers(mixin, memberFilter)
                        .ToList();

                    //Save Members
                    manager.CodeGenerationPlans[target].MixinGenerationPlans[mixin].Members = mixinMembers;
                }
            }

            return true;
        }

        private IEnumerable<MemberWrapper> CollectMemberWrappers(
            pMixinAttributeResolvedResult mixinAttribute,
            Func<IMember, bool> memberFilter,
            IType declaringType = null)
        {
            declaringType =
                declaringType ?? mixinAttribute.Mixin;

            if (declaringType.FullName.ToLower().Equals("system.object"))
                return Enumerable.Empty<MemberWrapper>();

            var allMembers =
               declaringType.GetMembers()
                   .Where(memberFilter)
                   .Select(m =>
                       new MemberWrapper
                       {
                           DeclaringType = mixinAttribute.Mixin,

                           Member = m,

                           MixinAttribute = mixinAttribute,

                           ParentDeclarations = 
                                CollectParentDeclarations(
                                    m,
                                    declaringType.DirectBaseTypes,
                                    mixinAttribute)

                       })
                    .ToList();

            //filter out parent members from list
            var allParentMembers =
                allMembers.SelectMany(x => x.ParentDeclarations)
                .ToList();

            allMembers =
                allMembers
                    .Where(m => !allParentMembers.Contains(
                        m,
                        new MemberWrapperExtensions.MemberWrapperEqualityComparer
                        {
                            IncludeDeclaringTypeInComparison = true
                        }
                    ))
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

        private IEnumerable<MemberWrapper> CollectParentDeclarations(
            IMember member,
            IEnumerable<IType> directBaseTypes,
            pMixinAttributeResolvedResult mixinAttribute)
        {
            return
                directBaseTypes
                    .Where(t => !t.FullName.ToLower().Equals("system.object"))
                    .Where(t => t.DefinesMember(member))
                    .Where(t => !Equals(t, member.DeclaringType))
                    .Select(t =>
                        new MemberWrapper
                        {
                            DeclaringType = t,

                            Member = t.GetDeclaredMemberThatMatchesSignature(member),

                            MixinAttribute = mixinAttribute,

                            ParentDeclarations =
                                CollectParentDeclarations(
                                    member,
                                    t.DirectBaseTypes,
                                    mixinAttribute)
                        });
        }
        
        private Func<IMember, bool> BuildMemberFilterFunction(
            pMixinAttributeResolvedResult mixinAttribute,
            ICompilation compilation)
        {
            var includeInternalMembers =
                    mixinAttribute.Mixin.GetDefinition().ParentAssembly.Equals(
                    compilation.MainAssembly);

            var doNotMixinIType =
                typeof(DoNotMixinAttribute)
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
