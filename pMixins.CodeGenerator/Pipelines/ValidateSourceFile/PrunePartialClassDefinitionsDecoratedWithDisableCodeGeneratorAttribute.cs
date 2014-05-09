//----------------------------------------------------------------------- 
// <copyright file="PrunePartialClassDefinitionsDecoratedWithDisableCodeGeneratorAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 9, 2014 4:19:53 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ValidateSourceFile
{
    public class PrunePartialClassDefinitionsDecoratedWithDisableCodeGeneratorAttribute :
        IPipelineStep<IParseSourceFilePipelineState>
    {
        public bool PerformTask(IParseSourceFilePipelineState manager)
        {
            var classesToPrune =
                manager.SourcePartialClassAttributes
                    .Where(x => x.Value.Any(a => TypeExtensions.Implements<DisableCodeGenerationAttribute>(a.AttributeType)))
                    .Select(x => x.Key)
                    .ToArray();

            if (0 == classesToPrune.Length)
                return true;

            //Log
            foreach (var classDef in classesToPrune)
                manager.CodeGenerationErrors.Add(
                    new CodeGenerationError(
                        string.Format(
                            Strings.MessageDisableCodeGenerationAttributePresentOnClass,
                            classDef.Name),
                        CodeGenerationError.SeverityOptions.Message,
                        line: classDef.StartLocation.Line,
                        column: classDef.StartLocation.Column));

            //Prune
            foreach (var classDef in classesToPrune)
            {
                manager.SourcePartialClassAttributes.Remove(classDef);
                manager.SourcePartialClassDefinitions.Remove(classDef);
            }

            return true;
        }
    }
}
