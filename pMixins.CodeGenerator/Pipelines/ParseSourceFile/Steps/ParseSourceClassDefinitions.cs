//----------------------------------------------------------------------- 
// <copyright file="ParseSourceClassDefinitions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, January 13, 2014 4:39:58 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile.Steps
{
    public class ParseSourceClassDefinitions : IPipelineStep<IParseSourceFilePipelineState>
    {
        public bool PerformTask(IParseSourceFilePipelineState manager)
        {
            foreach (var partialClassDef in manager.Context.Source.SyntaxTree
                                                        .GetPartialClasses())
            {
                manager.SourcePartialClassDefinitions.Add(partialClassDef);
            }

            return true;
        }
    }
}
