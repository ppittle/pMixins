//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorDependencyFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 13, 2014 5:09:17 PM</date> 
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
using ICSharpCode.NRefactory.TypeSystem;
using JetBrains.Annotations;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    public interface ICodeGeneratorDependencyFactory
    {
        [CanBeNull]
        CodeGeneratorDependency BuildDependency(CodeGeneratorResponse response);
    }

    public abstract class CodeGeneratorDependencyFactory<T> : ICodeGeneratorDependencyFactory
        where T : CodeGeneratorResponse
    {
        public virtual CodeGeneratorDependency BuildDependency(T response)
        {
            if (null == response ||
                null == response.CodeGeneratorContext)
                return null;

            var typeDependencies = GetTypeDependencies(response).ToList();

            return new CodeGeneratorDependency
            {
                TargetFile = response.CodeGeneratorContext.Source,

                TypeDependencies = typeDependencies,

                FileDependencies =
                    typeDependencies
                        .Select(t =>
                            response.CodeGeneratorContext.Solution.FindFileForIType(t))
                        .Where(f => null != f && 
                            !f.FileName.Equals(
                                response.CodeGeneratorContext.Source.FileName))
                        .ToList()
            };
        }

        protected abstract IEnumerable<IType> GetTypeDependencies(T response);
      
        CodeGeneratorDependency ICodeGeneratorDependencyFactory.BuildDependency(CodeGeneratorResponse response)
        {
            return BuildDependency(response as T);
        }
    }
}
