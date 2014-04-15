using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class IMethodExtenstions
    {
        public static string GetGenericMethodConstraints(this IMethod method, ICompilation compilation)
        {
            var sb = new StringBuilder();

            foreach (var typeParam in method.Parts.SelectMany(p =>
                p.TypeParameters.OfType<DefaultUnresolvedTypeParameter>()))
            {
                var resolvedParts = new List<string>();

                if (typeParam.HasDefaultConstructorConstraint)
                    resolvedParts.Add("new()");

                if (typeParam.HasReferenceTypeConstraint)
                    resolvedParts.Add("class");

                if (typeParam.HasValueTypeConstraint)
                    resolvedParts.Add("struct");

                resolvedParts.AddRange(
                    typeParam.Constraints.Resolve(compilation.TypeResolveContext)
                        .Select(x => x.GetOriginalFullName()));

                if (resolvedParts.Any())
                    sb.Append("where ").Append(typeParam.Name).Append(" : ").Append(string.Join(",", resolvedParts));
            }

            return sb.ToString();
        }

        public static string GetReturnString(this IMethod method)
        {
            return (null == method || method.ReturnType.Kind == TypeKind.Void)
                ? ""
                : "return";
        }
    }
}


