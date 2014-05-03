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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Infrastructure;

namespace CopaceticSoftware.CodeGenerator.StarterKit
{
    public interface ICodeGeneratorContextFactory
    {
        IEnumerable<ICodeGeneratorContext> GenerateContext(IEnumerable<RawSourceFile> rawSourceFiles);
    }

    public class CodeGeneratorContextFactory : ICodeGeneratorContextFactory
    {
        private readonly ISolutionManager _solutionManager;

        public CodeGeneratorContextFactory(ISolutionManager solutionManager)
        {
            Ensure.ArgumentNotNull(solutionManager, "solutionManager");

            _solutionManager = solutionManager;
        }

        public IEnumerable<ICodeGeneratorContext> GenerateContext(IEnumerable<RawSourceFile> rawSourceFiles)
        {
            if (null == rawSourceFiles)
                return Enumerable.Empty<ICodeGeneratorContext>();

            return
                _solutionManager.LoadCSharpFiles(rawSourceFiles)
                    .Select(
                        csharpFile =>
                            new CodeGeneratorContext
                            {
                                Source = csharpFile,
                                TypeResolver = csharpFile.CreateResolver()
                            });
        }
    }
}
