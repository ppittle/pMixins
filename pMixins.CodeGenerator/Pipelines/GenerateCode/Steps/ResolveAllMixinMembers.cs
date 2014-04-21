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
using JetBrains.Annotations;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps
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
            var rawMembers =
                manager.BaseState.PartialClassLevelResolvedpMixinAttributes[manager.SourceClass]
                    .OfType<pMixinAttributeResolvedResult>()
                    .ToDictionary(
                        mixinAttribute => mixinAttribute.Mixin,
                        mixinAttribute => ResolveMixinMembers(mixinAttribute, manager).ToList());

            var collisionFreeMethods = FilterOutCollisions(rawMembers);

            manager.MixinMembers = rawMembers;


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
                    resolvedMembers.Add(new MixinMemberResolvedResult{Member = mixinMember, ExplicitImplementationInterfaceType = maskMatch.Key});
            }

            return resolvedMembers;
        }

        private IDictionary<IType, List<MixinMemberResolvedResult>> FilterOutCollisions(
            IDictionary<IType, List<MixinMemberResolvedResult>> rawMembers)
        {
            for (int i = 1; i < rawMembers.Keys.Count; i++)
            {
                var iKey = rawMembers.Keys.ElementAt(i);

                for (int j = 0; j < i; j++)
                {
                    var jKey = rawMembers.Keys.ElementAt(j);

                    for (int m = 0; m < rawMembers[jKey].Count(); m++)
                    {
                        var memberUnderTest = rawMembers[jKey][m];

                        //collision test
                        if (rawMembers[iKey].Any(x => x.Member.EqualsMember(memberUnderTest.Member)))
                        {
                            //member collision
                            rawMembers[jKey][m] = ResolveCollision(jKey, rawMembers[jKey][m]);
                        }
                    }

                    rawMembers[jKey] = rawMembers[jKey].Where(x => null != x).ToList();
                }
            }

            return rawMembers;
        }

        [CanBeNull]
        private MixinMemberResolvedResult ResolveCollision(IType type, MixinMemberResolvedResult member)
        {
            if (member.Member is IField)
                //can't do anything for fields
                return null;

            var interfaces = type.GetAllBaseTypes()
                                .Where(bt => bt.Kind == TypeKind.Interface)
                                .ToList();

            var closestInterface =
                interfaces.FirstOrDefault(i => i.GetMembers().Any(m => m.EqualsMember(member.Member)));

            if (member.Member is IMethod)
            {
                var method = member.Member as IMethod;

                if (method.IsExplicitInterfaceImplementation)
                    //can't do anything for explicit methods
                    return null;

                //TODO

                //if (interfaces.Any())
            }

            return null;
        }
    }
}
