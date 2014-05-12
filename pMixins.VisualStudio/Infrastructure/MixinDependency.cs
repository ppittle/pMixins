//----------------------------------------------------------------------- 
// <copyright file="MixinDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 1:47:12 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.CodeGenerator;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.VisualStudio.Infrastructure
{
    public class MixinDependency
    {
        public CSharpFile TargetFile { get; private set; }

        public List<CSharpFile> FileDependencies { get; private set; }

        public List<IType> MixinTypeDependencies { get; private set; }

        public MixinDependency(pMixinPartialCodeGeneratorResponse response)
        {
            TargetFile = response.CodeGeneratorContext.Source;
            
            MixinTypeDependencies = GetTypeDependencies(response).ToList();

            FileDependencies =
                MixinTypeDependencies
                    .Select(t =>
                        response.CodeGeneratorContext.Solution.FindFileForIType(t))
                    .Where( f => null != f)
                    .ToList();
        }

        private IEnumerable<IType> GetTypeDependencies(pMixinPartialCodeGeneratorResponse response)
        {
            var classMixinAttributes = response.CodeGeneratorPipelineState.PartialClassLevelResolvedpMixinAttributes;

            foreach (var partialClass in classMixinAttributes.Keys)
            {
                foreach (
                    var pMixinResolvedResult in
                        classMixinAttributes[partialClass].OfType<pMixinAttributeResolvedResult>())
                {
                    yield return pMixinResolvedResult.Mixin;

                    if (null != pMixinResolvedResult.Interceptors)
                        foreach (var interceptor in pMixinResolvedResult.Interceptors)
                            yield return interceptor; 
                }
            }
        }
        
    }
}
