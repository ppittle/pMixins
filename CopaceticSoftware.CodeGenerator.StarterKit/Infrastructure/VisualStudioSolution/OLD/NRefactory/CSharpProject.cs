//----------------------------------------------------------------------- 
// <copyright file="CSharpProject.cs" company="Copacetic Software"> 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD.NRefactory
{
    /// <summary>
    /// Represents a C# project (.csproj file)
    /// </summary>
    /// <remarks>
    /// Copied from the NRefactory StringIndexOf sample
    /// http://www.codeproject.com/Articles/408663/Using-NRefactory-for-analyzing-Csharp-code
    /// </remarks>
    [DebuggerDisplay("{ToString()}")]
    public class CSharpProject
    {
        public class AssemblyReferencesResolved : EventArgs
        {
            public IList<string> References { get; set; } 
        }

        public event EventHandler<AssemblyReferencesResolved> OnAssemblyReferencesResolved;

        #region Properties
        /// <summary>
        /// Parent solution.
        /// </summary>
        private readonly Solution _solution;

        /// <summary>
        /// Title is the project name as specified in the .sln file.
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// Name of the output assembly.
        /// </summary>
        public readonly string AssemblyName;

        /// <summary>
        /// Full path to the .csproj file.
        /// </summary>
        public readonly string FileName;

        public readonly List<CSharpFile> Files = new List<CSharpFile>();

        public readonly CompilerSettings CompilerSettings = new CompilerSettings();

        /// <summary>
        /// The unresolved type system for this project.
        /// </summary>
        public IProjectContent ProjectContent { get; private set; }

        /// <summary>
        /// The resolved type system for this project.
        /// This field is initialized once all projects have been loaded (in Solution constructor).
        /// </summary>
        public ICompilation Compilation;
        #endregion

        public CSharpProject(Solution solution, string title, string fileName)
        {
            // Normalize the file name
            fileName = Path.GetFullPath(fileName);

            _solution = solution;
            Title = title;
            FileName = fileName;

            // Use MSBuild to open the .csproj
            var msbuildProject = GetMSBuildProject(fileName);

            // Figure out some compiler settings
            AssemblyName = msbuildProject.GetPropertyValue("AssemblyName");
            CompilerSettings.AllowUnsafeBlocks = GetBoolProperty(msbuildProject, "AllowUnsafeBlocks") ?? false;
            CompilerSettings.CheckForOverflow = GetBoolProperty(msbuildProject, "CheckForOverflowUnderflow") ?? false;
            string defineConstants = msbuildProject.GetPropertyValue("DefineConstants");
            foreach (string symbol in defineConstants.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                CompilerSettings.ConditionalSymbols.Add(symbol.Trim());

            // Initialize the unresolved type system
            ProjectContent = new CSharpProjectContent();
            ProjectContent = ProjectContent.SetAssemblyName(AssemblyName);
            ProjectContent = ProjectContent.SetProjectFileName(fileName);
            ProjectContent = ProjectContent.SetCompilerSettings(CompilerSettings);
            // Parse the C# code files
            foreach (var item in msbuildProject.GetItems("Compile"))
            {
                AddCSharpFile(
                    new CSharpFile(this, Path.Combine(msbuildProject.DirectoryPath, item.EvaluatedInclude)));
            }

            var assemblyReferences =
                new AssemblyReferencesResolved
                {
                    References = ResolveAssemblyReferences(msbuildProject).ToList()
                };
            
            if (null != OnAssemblyReferencesResolved)
                OnAssemblyReferencesResolved(this, assemblyReferences);

            // Add referenced assemblies:
            foreach (string assemblyFile in assemblyReferences.References)
            {
                IUnresolvedAssembly assembly = solution.LoadAssembly(assemblyFile);
                ProjectContent = ProjectContent.AddAssemblyReferences(new IAssemblyReference[] { assembly });
            }

            // Add project references:
            foreach (var item in msbuildProject.GetItems("ProjectReference"))
            {
                string referencedFileName = Path.Combine(msbuildProject.DirectoryPath, item.EvaluatedInclude);
                // Normalize the path; this is required to match the name with the referenced project's file name
                referencedFileName = Path.GetFullPath(referencedFileName);
                ProjectContent = ProjectContent.AddAssemblyReferences(new IAssemblyReference[] { new ProjectReference(referencedFileName) });
            }
        }

        private Project GetMSBuildProject(string fileName)
        {
            var loadedProjects = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(fileName);
            
            return loadedProjects.Any()
                ? loadedProjects.First()
                : new Project(fileName);
        }

        public CSharpFile AddCSharpFile(CSharpFile file)
        {
            Files.Add(file);

            // Add parsed files to the type system
            ProjectContent = ProjectContent.AddOrUpdateFiles(Files.Select(f => f.UnresolvedTypeSystemForFile));

            return file;
        }

        IEnumerable<string> ResolveAssemblyReferences(Project project)
        {
            // Use MSBuild to figure out the full path of the referenced assemblies
            var projectInstance = project.CreateProjectInstance();
            projectInstance.SetProperty("BuildingProject", "false");
            project.SetProperty("DesignTimeBuild", "true");

            projectInstance.Build("ResolveAssemblyReferences", new[] { new ConsoleLogger(LoggerVerbosity.Minimal) });
            var items = projectInstance.GetItems("_ResolveAssemblyReferenceResolvedFiles");
            string baseDirectory = Path.GetDirectoryName(FileName);
            return items.Select(i => Path.Combine(baseDirectory, i.GetMetadataValue("Identity")));
        }

        static bool? GetBoolProperty(Project p, string propertyName)
        {
            string val = p.GetPropertyValue(propertyName);
            bool result;
            if (bool.TryParse(val, out result))
                return result;

            return null;
        }

        public override string ToString()
        {
            return string.Format("[CSharpProject AssemblyName={0}]", AssemblyName);
        }

    }
}
