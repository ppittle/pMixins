//----------------------------------------------------------------------- 
// <copyright file="GenerateMixinContainerClassConstructor.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, January 28, 2014 12:42:22 AM</date> 
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
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Patterns;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PostClassGeneration
{
    public class GenerateMixinsContainerClassConstructor : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public const string MixinContainerConstructorParameterName = "host";

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            manager.MixinContainerClassGeneratorProxy
                .CreateConstructor(
                    "public",
                    new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(
                                manager.GeneratedClass.ClassName,
                                MixinContainerConstructorParameterName)
                        },
                      "",
                    string.Join(" ",
                                manager.MixinContainerClassConstructorStatements
                                    .Select(s => s.EnsureEndsWith(";")))
                );

            return true;
        }
    }
}
