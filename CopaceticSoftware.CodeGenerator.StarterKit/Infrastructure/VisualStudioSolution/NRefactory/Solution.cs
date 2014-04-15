//----------------------------------------------------------------------- 
// <copyright file="Solution.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, November 10, 2013 12:26:14 AM</date> 
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.NRefactory
{
    
    /// <summary>
    /// Represents a Visual Studio solution (.sln file).
    /// </summary>
    /// <remarks>
    /// Copied from the NRefactory StringIndexOf sample
    /// http://www.codeproject.com/Articles/408663/Using-NRefactory-for-analyzing-Csharp-code
    /// </remarks>
    [DebuggerDisplay("{FileName} - {Projects.Count} Projects")]
    public class Solution
    {
        public readonly string FileName;
        public readonly string Directory;
        public readonly List<CSharpProject> Projects = new List<CSharpProject>();

        public string FullName
        {
            get { return Path.Combine(Directory, FileName); }
        }

        public IEnumerable<CSharpFile> AllFiles
        {
            get
            {
                return Projects.SelectMany(p => p.Files);
            }
        }

        public Solution(string fileName)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
                throw new ArgumentException("File does not exist: " + fileName, "fileName");


            Directory = Path.GetDirectoryName(fileName);
            FileName = Path.GetFileName(fileName);

            var projectLinePattern = new Regex("Project\\(\"(?<TypeGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"");
            foreach (string line in File.ReadLines(fileName))
            {
                Match match = projectLinePattern.Match(line);
                if (match.Success)
                {
                    string typeGuid = match.Groups["TypeGuid"].Value;
                    string title = match.Groups["Title"].Value;
                    string location = match.Groups["Location"].Value;
                    string guid = match.Groups["Guid"].Value;
                    switch (typeGuid.ToUpperInvariant())
                    {
                        case "{2150E333-8FDC-42A3-9474-1A3956D46DE8}": // Solution Folder
                            // ignore folders
                            break;
                        case "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}": // C# project
                            Projects.Add(new CSharpProject(this, title, Path.Combine(Directory, location)));
                            break;
                        default:
                            Console.WriteLine("Project {0} has unsupported type {1}", location, typeGuid);
                            break;
                    }
                }
            }
            // Create compilations (resolved type systems) after all projects have been loaded.
            // (we can't do this earlier because project A might have a reference to project B, where A is loaded before B)
            // To allow NRefactory to resolve project references, we need to use a 'DefaultSolutionSnapshot'
            // instead of calling CreateCompilation() on each project individually.
            // Note - PJ Moved this to dedicated method.
            RecreateCompilations();

        }

        /// <summary>
        /// TODO: This is an expensive call - create a mechanism to suppress so a Compilations aren't recreated
        /// multiple times if multiple files are being added
        /// </summary>
        public void RecreateCompilations()
        {
            var solutionSnapshot = new DefaultSolutionSnapshot(Projects.Select(p => p.ProjectContent));

            foreach (CSharpProject project in Projects)
            {
                project.Compilation = solutionSnapshot.GetCompilation(project.ProjectContent);
            }

        }

        ConcurrentDictionary<string, IUnresolvedAssembly> assemblyDict =
            new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);

        /// <summary>
        /// Loads a referenced assembly from a .dll.
        /// Returns the existing instance if the assembly was already loaded.
        /// </summary>
        public IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            return assemblyDict.GetOrAdd(assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }
    }
}
