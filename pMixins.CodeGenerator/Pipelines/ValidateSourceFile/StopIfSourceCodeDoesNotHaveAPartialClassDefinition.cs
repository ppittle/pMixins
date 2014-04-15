//----------------------------------------------------------------------- 
// <copyright file="StopIfSourceCodeDoesNotHaveAPartialClassDefinition.cs" company="Copacetic Software"> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ValidateSourceFile
{
    public class StopIfSourceCodeDoesNotHaveAPartialClassDefinition : IPipelineStep<IParseSourceFilePipelineState>
    {
        public bool PerformTask(IParseSourceFilePipelineState manager)
        {
            if (manager.SourcePartialClassDefinitions.Any())
                return true;

            if (manager.Context.Source.SyntaxTree.GetClassDefinitions().Any())
                manager.CodeGenerationErrors.Add(new CodeGenerationError(Strings.WarningNoPartialClassInSourceFile));

            //At this point there's no classes to work on, so short-circuit the pipeline
            return false;
        }
    }
}
