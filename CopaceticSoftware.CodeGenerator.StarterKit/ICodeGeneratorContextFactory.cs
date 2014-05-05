//----------------------------------------------------------------------- 
// <copyright file="ICodeGeneratorContextFactory.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Infrastructure;

namespace CopaceticSoftware.CodeGenerator.StarterKit
{
    public interface ICodeGeneratorContextFactory
    {
        IList<ICodeGeneratorContext> GenerateContext(IList<RawSourceFile> rawSourceFiles);

        IList<ICodeGeneratorContext> GenerateContext(IEnumerable<CSharpFile> cSharpFiles);
    }

    public class CodeGeneratorContextFactory : ICodeGeneratorContextFactory
    {
        private readonly ISolutionFactory _solutionFactory;

        public CodeGeneratorContextFactory(ISolutionFactory solutionFactory)
        {
            _solutionFactory = solutionFactory;
        }

        public IList<ICodeGeneratorContext> GenerateContext(IList<RawSourceFile> rawSourceFiles)
        {
            if (null == rawSourceFiles)
                return new List<ICodeGeneratorContext>();

            var solution = _solutionFactory.BuildCurrentSolution();

            if (null == solution)
                return new List<ICodeGeneratorContext>();

            var csharpFiles = 
                rawSourceFiles
                .Select(f => solution.AddOrUpdateProjectItemFile(f))
                .ToList();

            //This causes attributes to be listed twice.

            solution.RecreateCompilations();
            return GenerateContext(solution, csharpFiles);
        }

        public IList<ICodeGeneratorContext> GenerateContext(IEnumerable<CSharpFile> cSharpFiles)
        {
            if (null == cSharpFiles)
                return new List<ICodeGeneratorContext>();

            return GenerateContext(_solutionFactory.BuildCurrentSolution(), cSharpFiles);
        }

        private IList<ICodeGeneratorContext> GenerateContext(Solution s, IEnumerable<CSharpFile> cSharpFiles)
        {

            return
                cSharpFiles
                    //Load the file from the solution (so it has the latest compilation)
                    .Select(x => s.AllFiles.FirstOrDefault(f => f.FileName.Equals(x.FileName)))
                    .Where(x => null != x)
                    .Select(f => (ICodeGeneratorContext)new CodeGeneratorContext
                    {
                        Source = f,
                        TypeResolver = f.CreateResolver()
                    })
                    .ToList();
        }
    }
}
