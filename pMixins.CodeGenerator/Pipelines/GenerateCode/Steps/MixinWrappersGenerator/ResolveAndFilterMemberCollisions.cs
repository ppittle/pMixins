//----------------------------------------------------------------------- 
// <copyright file="ResolveAndFilterMemberCollisions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 12:02:29 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration;
using ICSharpCode.NRefactory.TypeSystem;
using JetBrains.Annotations;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator
{
    /// <summary>
    /// After <see cref="ResolveAllMixinMembers"/> in the <see cref="pMixinGenerator"/> Preamble,
    /// this step needs to run to ensure duplicate members are not added to the
    /// Generated Target Class.  Therefore, this should run first in the 
    /// <see cref="pMixinGenerator._targetPartialClassGenerator"/> step.
    /// </summary>
    /// <remarks>
    /// This is not run in the <see cref="pMixinGenerator._mixinWrapperGeneratorPipeline"/> as 
    /// the wrappers are Mixin specific and can safely implement all methods.  This is especially
    /// necessary in the case of duplicate abstract methods:
    /// <code>
    /// <![CDATA[
    /// public abstract class BaseClass{
    ///     public abstract string Prop{get;}
    /// }
    /// 
    /// public abstract class Child1 : BaseClass{}
    /// 
    /// public abstract class Child2 : BaseClass{}
    /// 
    /// [pMixin(Mixin = typeof(Child1)]
    /// [pMixin(Mixin = typeof(Child2)]
    /// public partial class Target{}
    /// ]]> 
    /// </code>
    /// In this case, Abstract Wrappers for both Child1 and Child2 will need to implement
    /// Prop, but Target can only define it once.  In this case, Child1.Prop is proxied.
    /// </remarks>
    public class ResolveAndFilterMemberCollisions : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            manager.CurrentMixinMembers =
                FilterOutCollisions(
                    manager.MixinMembers,
                    manager.CurrentpMixinAttribute.Mixin,
                    manager.CurrentMixinMembers.ToList());

            return true;
        }

        private IEnumerable<MixinMemberResolvedResult> FilterOutCollisions(
            Dictionary<IType, List<MixinMemberResolvedResult>> allMixinMembers,
            IType currentMixin,
            IList<MixinMemberResolvedResult> currentMixinMembers)
        {
            //if there is only one mixin, then we don't have to worry 
            //about collisions
            if (allMixinMembers.Keys.Count == 1)
                return currentMixinMembers;

            //if currentMixin is the first mixin then we don't have to 
            //worry about collisions.  The other mixins will need to prevent
            //the collision.
            if (allMixinMembers.Keys.First().Equals(currentMixin))
                return currentMixinMembers;

            //Where is the currentMixin in the line
            //of all mixins
            var indexOfCurrentMixin = allMixinMembers.Keys.IndexOf(x => x.Equals(currentMixin));

            var mixinsAheadInLineMembers = 
                allMixinMembers
                    .Take(indexOfCurrentMixin)
                    .SelectMany(x => x.Value)
                    .ToList();
            
            for (var currentMixinMemberIndex = 0;
                currentMixinMemberIndex < currentMixinMembers.Count();
                currentMixinMemberIndex++)
            {
                var currentMixinMethodUnderTest = currentMixinMembers[currentMixinMemberIndex];

                if (mixinsAheadInLineMembers.Any(x => x.Member.EqualsMember(currentMixinMethodUnderTest.Member)))
                {
                    currentMixinMembers[currentMixinMemberIndex] = ResolveCollision(currentMixin,
                        currentMixinMethodUnderTest);
                }
            }

            //Remove any member where ResolveCollision set the resolution to null
            return currentMixinMembers.Where(x => null != x);
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
