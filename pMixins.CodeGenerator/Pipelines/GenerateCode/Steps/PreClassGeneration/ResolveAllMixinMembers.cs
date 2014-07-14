//----------------------------------------------------------------------- 
// <copyright file="ResolveAllMixinMembers.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 7, 2014 8:50:16 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration
{
    /// <summary>
    /// Iterate through all Mixins and collect the <see cref="IMember"/>s
    /// that will be added to the Target.  Populates <see cref="pMixinGeneratorPipelineState.MixinMembers"/>
    /// 
    /// Figure out if a <see cref="IMember"/> needs to be implemented explicitly 
    /// or not mixed in at all because of an inevitable conflict.
    /// </summary>
    public class ResolveAllMixinMembers : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            manager.MixinMembers = 
                manager.BaseState.PartialClassLevelResolvedpMixinAttributes[manager.SourceClass]
                    .OfType<pMixinAttributeResolvedResult>()
                    .ToDictionary(
                        mixinAttribute => mixinAttribute.Mixin,
                        mixinAttribute => ResolveMixinMembers(mixinAttribute, manager).ToList());
            
            return true;
        }

        private IEnumerable<MixinMemberResolvedResult> ResolveMixinMembers(
            pMixinAttributeResolvedResult mixinAttribute, pMixinGeneratorPipelineState manager)
        {
            var includeInternalMembers =
                    mixinAttribute.Mixin.GetDefinition().ParentAssembly.Equals(
                    manager.BaseState.Context.TypeResolver.Compilation.MainAssembly);

            var doNotMixinIType = 
                typeof (DoNotMixinAttribute)
                .ToIType(manager.BaseState.Context.TypeResolver.Compilation);

            var memberFilter = new Func<IMember, bool>(
                member => ( 
                            !member.IsPrivate && 
                            (!member.IsProtected || !mixinAttribute.Mixin.GetDefinition().IsSealed) &&
                            (!member.IsInternal || includeInternalMembers) &&
                            !member.FullName.StartsWith("System.Object") &&
                            !member.IsDecoratedWithAttribute(doNotMixinIType)));

            //If no masks, just return mixin's members
            if (null == mixinAttribute.Masks || !mixinAttribute.Masks.Any())
                return mixinAttribute.Mixin.GetMembers().Where(memberFilter)
                    .Select(member => new MixinMemberResolvedResult {Member = member});


            //There are masks, so generate allowed members by mask
            var maskMethods =
                mixinAttribute.Masks
                .ToDictionary(mask => mask, mask => mask.GetMembers().Where(memberFilter));

            var resolvedMembers = new List<MixinMemberResolvedResult>();

            foreach (var mixinMember in mixinAttribute.Mixin.GetMembers().Where(memberFilter))
            {
                var maskMatch =
                    maskMethods.FirstOrDefault(
                        mask => mask.Value.Any(member => mixinMember.EqualsMember(member)));
                
                if (null != maskMatch.Key)
                    resolvedMembers.Add(
                        new MixinMemberResolvedResult
                        {
                            Member = mixinMember, 
                            ExplicitImplementationInterfaceType = maskMatch.Key
                        });
            }

            return resolvedMembers;
        }
        
    }
}
