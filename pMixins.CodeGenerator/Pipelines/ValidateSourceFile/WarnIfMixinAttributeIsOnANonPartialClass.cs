//----------------------------------------------------------------------- 
// <copyright file="WarnIfMixinAttributeIsOnANonPartialClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, January 26, 2014 12:03:09 AM</date> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ValidateSourceFile
{
    public class WarnIfMixinAttributeIsOnANonPartialClass : IPipelineStep<IParseSourceFilePipelineState>
    {
        public bool PerformTask(IParseSourceFilePipelineState manager)
        {
            foreach (var nonPartialClassDef in manager.Context.Source.SyntaxTree
                .GetClassDefinitions()
                .Where(x => !x.HasModifier(Modifiers.Partial)))
            {
                var resolvedClass = manager.Context.TypeResolver.Resolve(nonPartialClassDef).Type;

                if (resolvedClass.GetAttributes(false).Any(x => x.AttributeType.Implements<pMixins.Attributes.pMixinAttribute>()))
                {
                    manager.CodeGenerationErrors.Add(
                        new CodeGenerationError(
                            string.Format(
                                Strings.WarningpMixinAttributeOnNonPartialClass,
                                nonPartialClassDef.Name),
                            CodeGenerationError.SeverityOptions.Warning,
                            nonPartialClassDef.StartLocation.Line,
                            nonPartialClassDef.StartLocation.Column));

                    //A warning has been logged, but execution can continue,
                    //do *not* return false
                }
            }

            return true;
        }
    }
}
