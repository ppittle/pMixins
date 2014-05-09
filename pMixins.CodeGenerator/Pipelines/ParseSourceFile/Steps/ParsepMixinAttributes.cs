//----------------------------------------------------------------------- 
// <copyright file="ParsepMixinAttributes.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ParseSourceFile.Steps
{
    public class ParsepMixinAttributes : IPipelineStep<IParseSourceFilePipelineState>
    {
        public bool PerformTask(IParseSourceFilePipelineState manager)
        {
            #region Assembly Attributes
            manager.AssemblyAttributes.AddRange(
                manager.Context.TypeResolver.Compilation.MainAssembly
                       .AssemblyAttributes
                       .Where(x => x.AttributeType.Implements<IPMixinAttribute>()));
            #endregion

            #region Class Attributes
            foreach (var classDef in manager.SourcePartialClassDefinitions)
            {
                manager.SourcePartialClassAttributes.Add(classDef, 
                    new List<IAttribute>());

                var resolvedClass = manager.Context.TypeResolver.Resolve(classDef);

                if (resolvedClass.IsError)
                    #region Log and Return False

                {
                    manager.CodeGenerationErrors.Add(
                        new CodeGenerationError
                            {
                                Message = string.Format(Strings.WarningFailedToResolveClass, classDef.Name),
                                Line = (uint) classDef.StartLocation.Line,
                                Column = (uint) classDef.StartLocation.Column,
                                Severity = CodeGenerationError.SeverityOptions.Warning
                            });

                    return false;
                }
                #endregion

                manager.SourcePartialClassAttributes[classDef].AddRange(
                        resolvedClass.Type
                        .GetAttributes()
                        .Where(x => x.AttributeType.Implements<IPMixinAttribute>()));
            }
            #endregion

            return true;
        }
    }
}
