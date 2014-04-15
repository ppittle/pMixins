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
    }
}