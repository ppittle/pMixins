using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class IParametersExtensions
    {
        public static IList<KeyValuePair<string, string>> ToKeyValuePair(this IEnumerable<IParameter> parameters)
        {
            if (null == parameters)
                return new List<KeyValuePair<string, string>>();

            return parameters.Select(p => new KeyValuePair<string, string>(p.Type.GetFullName(), p.Name)).ToList();
        }
    }
}
