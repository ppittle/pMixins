//----------------------------------------------------------------------- 
// <copyright file="MixinMembersHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 16, 2014 2:47:40 PM</date> 
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
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure
{
    public static class MixinMembersHelper
    {
        public static IEnumerable<IMember> GetUnimplementedAbstractMembers(this IList<MixinMemberResolvedResult> members)
        {
            return members
                .Select(x => x.Member)
                .Where(member => member.IsAbstract 
                    //make sure we don't have a concrete implementation somewhere
                    && members.Count(x => x.Member.EqualsMember(member)) == 1);
        }
    }
}
