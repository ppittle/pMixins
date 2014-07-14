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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class MemberExtensions
    {
        public static bool EqualsMember(this IMember member, IMember otherMember)
        {
            var equalityChecks = new Func<IMember, IMember, bool>[]
            {
                (m1, m2) => m1.Name == m2.Name,
                (m1, m2) => Equals(m1.ReturnType, m2.ReturnType),
                (m1, m2) =>
                {
                    if ((null != m1 as IMethod) && (null != m2 as IMethod))
                    {
                        var m1Meth = m1 as IMethod;
                        var m2Meth = m2 as IMethod;

                        if (m1Meth.TypeParameters.Count != m2Meth.TypeParameters.Count)
                            return false;

                        for (var i = 0; i < m1Meth.Parameters.Count; i++)
                        {
                            if (!Equals(m1Meth.Parameters[i].Type, m2Meth.Parameters[i].Type))
                                return false;
                        }
                    }

                    return true;
                }
            };

            return equalityChecks.All(check => check(member, otherMember));
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
    }
}
