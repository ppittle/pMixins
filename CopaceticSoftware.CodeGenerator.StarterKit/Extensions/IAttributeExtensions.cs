
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class IAttributeExtensions
    {
        public static object GetNamedArgumentValue(this IAttribute attribute,
            string attributePropertyName)
        {
            return attribute.NamedArguments
                            .Where(arg => arg.Key.Name == attributePropertyName)
                            .Select(arg => arg.Value.GetValue())
                            .FirstOrDefault();

        }

        public static IEnumerable<IAttribute> FilterOutNonInheritedAttributes(this IEnumerable<IAttribute> attributes)
        {
            foreach (var attr in attributes)
            {
                var attributeUsageAttr =
                    attr.AttributeType.GetAttributes(true, true)
                        .FirstOrDefault(a => a.AttributeType.FullName == "System.AttributeUsageAttribute");

                if (null == attributeUsageAttr)
                    yield return attr;

                if (!attributeUsageAttr.NamedArguments
                    .Any(na => null != na.Key && na.Key.Name == "Inherited"))
                {
                    yield return attr;
                }

                var inheritedArg = attributeUsageAttr.NamedArguments
                    .First(na => na.Key.Name == "Inherited");

                if (System.Convert.ToBoolean(inheritedArg.Value.ConstantValue) == true)
                    yield return attr;

                //else - it's not inherited.
            }
        }

        public static IEnumerable<Attribute> ConvertToAttributeAstTypes(this IEnumerable<IAttribute> attributes)
        {
            foreach (var attr in attributes)
            {
                var attributeAst = attr.AttributeType.ToAstSyntaxType();

                var attribute =
                    new Attribute
                    {
                        Type = attributeAst,
                        Role = Roles.Attribute
                    };

                foreach (var positionalArg in attr.PositionalArguments)
                {
                    attribute.Arguments.Add(new PrimitiveExpression(positionalArg.ConstantValue));
                }

                foreach (var namedArg in attr.NamedArguments)
                {
                    attribute.Arguments.Add(new NamedExpression(
                        namedArg.Key.Name,
                        new PrimitiveExpression(namedArg.Value.ConstantValue)));
                }

                yield return attribute;
            }
        }
    }
}
