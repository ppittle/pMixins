//----------------------------------------------------------------------- 
// <copyright file="Solution.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    [DebuggerDisplay("{File} - {Projects.Count} Projects")]
    public class Solution
    {
        public readonly string FileName;
        public readonly IEnumerable<CSharpProject> Projects;

        public Solution(string fileName, IEnumerable<CSharpProject> projects)
        {
            FileName = fileName;
            Projects = projects;
        }

        /// <summary>
        /// Sets <see cref="CSharpProject.Compilation"/> for every project in 
        /// <see cref="Projects"/>.  This is only valid once every project
        /// is fully loaded.
        /// </summary>
        public void RecreateCompilations()
        {
            var solutionSnapshot = new DefaultSolutionSnapshot(Projects.Select(p => p.ProjectContent));

            foreach (CSharpProject project in Projects)
            {
                project.Compilation = solutionSnapshot.GetCompilation(project.ProjectContent);
            }
        }
    }
}
