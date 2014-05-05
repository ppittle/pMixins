//----------------------------------------------------------------------- 
// <copyright file="SolutionFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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

using System.IO;
using System.Linq;
using CopaceticSoftware.Common.Infrastructure;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface ISolutionFactory
    {
        Solution BuildSolution(string solutionFileName);
    }

    public class SolutionFactory : ISolutionFactory
    {
        private readonly ISolutionFileReader _solutionFileReader;
        private readonly ICSharpProjectFactory _cSharpProjectFactory;

        public SolutionFactory(ISolutionFileReader solutionFileReader, ICSharpProjectFactory cSharpProjectFactory)
        {
            _solutionFileReader = solutionFileReader;
            _cSharpProjectFactory = cSharpProjectFactory;
        }

        public Solution BuildSolution(string solutionFileName)
        {
            Ensure.ArgumentNotNullOrEmpty(solutionFileName,"solutionFileName");
            
            return new Solution(
                solutionFileName,
                _solutionFileReader.ReadProjectReferences(solutionFileName)
                    .Select(pr => _cSharpProjectFactory.BuildProject(pr.ProjectFileName, pr.Title))
                );
        }
    }
}
