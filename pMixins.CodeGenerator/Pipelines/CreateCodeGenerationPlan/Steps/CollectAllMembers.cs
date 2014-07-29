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
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
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
                        //TODO: At this point should duplicates be removed or marked as 'don't implement'?
                        .DistinctMemberWrappers()
                        .ToList();

                    //Save Members
                    manager.CodeGenerationPlans[target].MixinGenerationPlans[mixin].Members = mixinMembers;
                }
            }

            return true;
        }

        private IEnumerable<MemberWrapper> CollectMemberWrappers(
            pMixinAttributeResolvedResult mixinAttribute,
            Func<IMember, bool> memberFilter)
        {
            return 
                //Collect all applicable types
                mixinAttribute.Mixin.GetAllBaseTypes()
                    .Union(new[] {mixinAttribute.Mixin})
                    //Collect all members
                    .SelectMany(t => CollectMemberWrappers(t, mixinAttribute, memberFilter));
        }

        private IEnumerable<MemberWrapper> CollectMemberWrappers(
            IType parentType, 
            pMixinAttributeResolvedResult mixinAttribute,
            Func<IMember, bool> memberFilter)
        {
            return
                parentType.GetMembers()
                    .Where(memberFilter)
                    .Select(m =>
                        new MemberWrapper
                        {
                            DeclaringType = parentType,
                            Member = m,
                            MixinAttribute = mixinAttribute
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
