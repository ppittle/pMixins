//----------------------------------------------------------------------- 
// <copyright file="MicrosoftBuildProjectAssemblyReferenceResolver.cs" company="Copacetic Software"> 
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface IMicrosoftBuildProjectAssemblyReferenceResolver
    {
        IEnumerable<IAssemblyReference> ResolveReferences(Project project, string projectFileName);
    }

    //Should be singleton
    public class MicrosoftBuildProjectAssemblyReferenceResolver : IMicrosoftBuildProjectAssemblyReferenceResolver
    {
        private readonly ConcurrentDictionary<string, IUnresolvedAssembly> _assemblyDict;

        public MicrosoftBuildProjectAssemblyReferenceResolver()
        {
            _assemblyDict = 
                new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);
        }


        public IEnumerable<IAssemblyReference> ResolveReferences(Project project, string projectFileName)
        {
            return ResolveAssemblyReferences(project, projectFileName)
                .Union<IAssemblyReference>(ResolvedProjectReferences(project));
        }

        protected virtual IEnumerable<IUnresolvedAssembly> ResolveAssemblyReferences(Project project, string projectFileName)
        {
            string baseDirectory = Path.GetDirectoryName(projectFileName);

            // Use MSBuild to figure out the full path of the referenced assemblies
            var projectInstance = project.CreateProjectInstance();
            projectInstance.SetProperty("BuildingProject", "false");
            project.SetProperty("DesignTimeBuild", "true");

            projectInstance.Build("ResolveAssemblyReferences", new[] { new ConsoleLogger(LoggerVerbosity.Minimal) });

            var items = projectInstance.GetItems("_ResolveAssemblyReferenceResolvedFiles");

            return items.Select(i => LoadAssembly(Path.Combine(baseDirectory, i.GetMetadataValue("Identity"))));
        }

        protected virtual IEnumerable<ProjectReference> ResolvedProjectReferences(Project project)
        {
            foreach (var item in project.GetItems("ProjectReference"))
            {
                string referencedFileName = Path.Combine(project.DirectoryPath, item.EvaluatedInclude);

                // Normalize the path; this is required to match the name with the referenced project's file name
                referencedFileName = Path.GetFullPath(referencedFileName);

                yield return new ProjectReference(referencedFileName);
            }
        }


        protected IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            return _assemblyDict.GetOrAdd(
                assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }
    }
}
