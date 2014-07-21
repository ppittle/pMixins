//----------------------------------------------------------------------- 
// <copyright file="IAttributeExtensions.cs" company="Copacetic Software"> 
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
                    .Where(na => na.Key.Name == "Inherited")
                    .ToList();

                if (inheritedArg.Count > 0 &&
                    System.Convert.ToBoolean(inheritedArg[0].Value.ConstantValue) == true)
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
