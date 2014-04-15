using System;
using System.Linq;
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.Semantics;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class ResolveResultExtensions
    {
        public static object GetValue(this ResolveResult result, bool ignoreError = false)
        {
            if (null == result || (result.IsError && !ignoreError))
                return null;

            object value = null;

            if (result.TryCast<UnknownIdentifierResolveResult>(r => value = r.Identifier))
                return value;

            if (result.TryCast<ConstantResolveResult>(r => value = r.ConstantValue))
                return value;

            if (result.TryCast<MemberResolveResult>(r => value = r.ConstantValue))
                return value;

            if (result.TryCast<TypeOfResolveResult>(r => value = r.ReferencedType))
                return value;

            if (result.TryCast<ConversionResolveResult>(r => value = r.Input.GetValue()))
                return value;

            if (result.TryCast<ArrayCreateResolveResult>(r =>
                                value = r.InitializerElements
                                    .Select(x => x.GetValue(ignoreError: true))
                                    .ToArray()))
                return value;

            throw new Exception("THIS IS A BUG: Do not know how to get Value from a ResolveResult: " + result.GetType().Name);
        }
    }
}
