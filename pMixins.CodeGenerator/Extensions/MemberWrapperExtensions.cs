﻿//----------------------------------------------------------------------- 
// <copyright file="MemberWrapperExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 14, 2014 1:13:43 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Extensions
{
    public static class MemberWrapperExtensions
    {
        private static readonly MemberWrapperEqualityComparer _equalityComparer = new MemberWrapperEqualityComparer();

        public class MemberWrapperEqualityComparer : IEqualityComparer<MemberWrapper>
        {
            public bool IncludeDeclaringTypeInComparison { get; set; }

            public bool Equals(MemberWrapper x, MemberWrapper y)
            {
                return new MemberExtensions.MemberEqualityComparer
                {
                    IncludeDeclaringTypeInComparison = IncludeDeclaringTypeInComparison
                }.Equals(x.Member, y.Member);
            }

            public int GetHashCode(MemberWrapper obj)
            {
                return new MemberExtensions.MemberEqualityComparer
                {
                    IncludeDeclaringTypeInComparison = IncludeDeclaringTypeInComparison,
                }.GetHashCode(obj.Member);
            }
        }

        public static IEnumerable<MemberWrapper> DistinctMemberWrappers(this IEnumerable<MemberWrapper> memberWrappers, bool includeDeclaringTypeInComparison = false)
        {
            return 
                memberWrappers.Distinct(new MemberWrapperEqualityComparer{IncludeDeclaringTypeInComparison = includeDeclaringTypeInComparison});
        }

        public static IEnumerable<MemberWrapper> FilterMemberWrappers(
            this IEnumerable<MemberWrapper> memberWrappers,
            IEnumerable<MemberWrapper> membersToExclude)
        {
            return memberWrappers.FilterMemberWrappers(membersToExclude.Select(x => x.Member));
        }

        public static IEnumerable<MemberWrapper> FilterMemberWrappers(
            this IEnumerable<MemberWrapper> memberWrappers,
            IEnumerable<IMember> membersToExclude)
        {
            return
                memberWrappers
                    .Where(mw => !membersToExclude.Contains(mw.Member, new MemberExtensions.MemberEqualityComparer()));
        }
    }
}
