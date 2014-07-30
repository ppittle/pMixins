//----------------------------------------------------------------------- 
// <copyright file="MemberExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 5:48:10 PM</date> 
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
using ICSharpCode.NRefactory.TypeSystem;
using Mono.CSharp;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class MemberExtensions
    {
        private static readonly MemberEqualityComparer _equalityComparer = new MemberEqualityComparer();
        public class MemberEqualityComparer : IEqualityComparer<IMember>
        {
            public bool Equals(IMember x, IMember y)
            {
                return GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode(IMember obj)
            {
                var hashCodeParts = new Func<IMember, string>[]
                {
                    //Hash member name
                    m => m.Name.GetHashCode().ToString(),
                    //Hash return type
                    m => m.ReturnType.GetHashCode().ToString(),
                    //Hash method parameters
                    m =>
                    {
                        var result = "";

                        var method = m as IMethod;

                        if (null == method)
                            return result;

                        result += method.TypeParameters.Count.GetHashCode();

                        foreach (var p in method.Parameters)
                            result += p.Type;

                        return result;
                    },
                    //Hash get vs set vs indexer on Property
                    m =>
                    {
                        var result = "";

                        var property = m as IProperty;

                        if (null == property)
                            return result;

                        if (property.CanGet)
                            result += "Get";

                        if (property.CanSet)
                            result += "Set";

                        if (property.IsIndexer)
                            result += "Indexer";

                        return result;
                    }
                        
                    
                };

                var hash = 
                    string.Join("", hashCodeParts.Select(x => x(obj))).GetHashCode();

                return hash;
            }
        }

        public static bool EqualsMember(this IMember member, IMember otherMember)
        {
            return _equalityComparer.Equals(member, otherMember);
        }

        public static IEnumerable<IMember> DistinctMembers(this IEnumerable<IMember> members)
        {
            return members.Distinct(_equalityComparer);
        }

        public static IEnumerable<KeyValuePair<IMember, int>> GroupByCount(this IEnumerable<IMember> members)
        {
            var membersList = members.ToList();

            var distinctMembers = membersList.DistinctMembers().ToList();

            return
                distinctMembers.Select(x => 
                        new KeyValuePair<IMember, int>(
                            x, 
                            membersList.Count(m => _equalityComparer.Equals(m, x))));
        }

        public static string GetOriginalName(this IMember member)
        {
            var method = member as IMethod;

            if (null == method)
                return member.Name;

            return !method.TypeParameters.Any()
                ? method.Name
                : string.Format(
                    "{0}<{1}>",
                    method.Name,
                    string.Join(",",
                        method.TypeParameters.Select(t => t.Name)
                        ));
        }

        public static bool IsDecoratedWithAttribute(this IMember member, IType attributeType)
        {
            return member.Attributes
                .Union(
                    member.ImplementedInterfaceMembers.SelectMany(x => x.DeclaringType.GetAttributes()))
                .Any(a => Equals(a.AttributeType, attributeType));
        }

        public static string GetModifiersString(this IMember member, string overrideModifiers = null )
        {
            if (null == overrideModifiers)
            {
                overrideModifiers =
                    (member.IsOverridable
                        ? " virtual"
                        : (member.IsOverride
                            ? " override"
                            : ""));
            }

            var additionalModifiers = overrideModifiers;

            additionalModifiers +=
                member.IsStatic
                    ? " static"
                    : "";

            var method = member as IMethod;

            if (null != method)
            {
                additionalModifiers +=
                    method.IsAsync
                        ? " async"
                        : "";
            }

            if (member.IsPrivate)
                return "private " + additionalModifiers;

            if (member.IsProtected)
                return "protected " + additionalModifiers;

            if (member.IsInternal)
                return "internal " + additionalModifiers;

            return "public " + additionalModifiers;
        }

        public static bool IsStaticOrConst(this IMember member)
        {
            return 
                member.IsStatic || 
                (member is IField && (member as IField).IsConst);
        }

    }
}
