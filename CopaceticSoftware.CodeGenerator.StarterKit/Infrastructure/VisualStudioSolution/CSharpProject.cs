//----------------------------------------------------------------------- 
// <copyright file="CSharpProject.cs" company="Copacetic Software"> 
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
using System.Linq;
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public class CSharpProject
    {
        #region Public Fields
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

        public readonly CompilerSettings CompilerSettings;

        public readonly List<CSharpFile> Files;

        /// <summary>
        /// The unresolved type system for this project.
        /// </summary>
        public readonly IProjectContent ProjectContent;

        /// <summary>
        /// The resolved type system for this project.
        /// This field is initialized externally.
        /// </summary>
        public ICompilation Compilation;
        #endregion

        public CSharpProject(
            MicrosoftBuildProject msBuildProject,
            string title)
        {
            Title = title;

            AssemblyName = msBuildProject.AssemblyName;
            FileName = msBuildProject.FileName;

            Files = msBuildProject.CompiledFileNames.Select(
                f => new CSharpFile(this, f)).ToList();

            CompilerSettings =
                #region new CompilerSettings
                new CompilerSettings
                {
                    AllowUnsafeBlocks = msBuildProject.AllowUnsafeBlocks,
                    CheckForOverflow = msBuildProject.CheckForOverflowUnderflow,
                };

            CompilerSettings.ConditionalSymbols.AddRange(msBuildProject.DefineConstants);
            #endregion

            ProjectContent = new CSharpProjectContent();

            ProjectContent.SetAssemblyName(msBuildProject.AssemblyName);
            ProjectContent.SetProjectFileName(msBuildProject.FileName);
            ProjectContent.SetCompilerSettings(CompilerSettings);

            ProjectContent = ProjectContent.AddOrUpdateFiles(
                Files.Select(f => f.UnresolvedTypeSystemForFile));

            ProjectContent = ProjectContent.AddAssemblyReferences(msBuildProject.ReferencedAssemblies);
        }
    }
}
