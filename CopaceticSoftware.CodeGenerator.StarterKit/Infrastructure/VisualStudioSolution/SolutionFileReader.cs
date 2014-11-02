//----------------------------------------------------------------------- 
// <copyright file="SolutionFileReader.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using EnvDTE;
using EnvDTE80;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public class SolutionFileProjectReference
    {
        public string Title;
        public FilePath ProjectFileName;
    }
    public interface ISolutionFileReader
    {
        IEnumerable<SolutionFileProjectReference> ReadProjectReferences(FilePath solutionFileName);
    }

    public class SolutionDTEReader : ISolutionFileReader
    {
        private readonly DTE2 _dte;

        public SolutionDTEReader(DTE2 dte)
        {
            _dte = dte;
        }

        public IEnumerable<SolutionFileProjectReference> ReadProjectReferences(FilePath solutionFileName)
        {
            var projectReferences = new List<SolutionFileProjectReference>();

            var allProjects = RecursivelyCollectProjects(_dte.Solution.Projects.OfType<Project>());
            
            foreach (var p in allProjects)
            {
                if (string.IsNullOrEmpty(p.FullName))
                    continue;

                projectReferences.Add(new SolutionFileProjectReference
                {
                    ProjectFileName = new FilePath(p.FullName),
                    Title = p.Name
                });
            }

            return projectReferences;
        }

      
        /// <summary>
        /// Collects CSharp Projects and takes into account Solution Folders.
        /// 
        /// Solution Folders will appear as a Project.
        /// http://weblogs.asp.net/soever/enumerating-projects-in-a-visual-studio-solution
        /// </summary>
        private IEnumerable<Project> RecursivelyCollectProjects(
            IEnumerable<Project> projects)
        {
            if (null == projects)
                yield break;
            
            foreach (var p in projects)
            {
                if (p.ConfigurationManager != null)
                {
                    //it's a project
                    yield return p;
                }
                else if (null != p.ProjectItems)
                {
                    foreach (var pi in p.ProjectItems.OfType<ProjectItem>())
                    {
                        if (null != pi.SubProject)
                        {
                            //pi is a solution folder
                            foreach (var childP in RecursivelyCollectProjects(
                                new []{pi.SubProject}))
                            {
                                yield return childP;
                            }
                        }
                    }
                }
            }
        }
    }

    public class SolutionFileReader : ISolutionFileReader
    {
        private readonly IFileReader _fileReader;
        private readonly IFileWrapper _fileWrapper;
       

        private static readonly Regex ProjectLinePattern = new Regex(
            "Project\\(\"(?<TypeGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"");

        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SolutionFileReader(IFileReader fileReader, IFileWrapper fileWrapper)
        {
            _fileReader = fileReader;
            _fileWrapper = fileWrapper;
        }

        public IEnumerable<SolutionFileProjectReference> ReadProjectReferences(FilePath solutionFileName)
        {
            if (!_fileWrapper.Exists(solutionFileName))
                throw new FileNotFoundException("Solution FileName", solutionFileName.FullPath);

            if (solutionFileName.IsNullOrEmpty())
                throw new ArgumentNullException("solutionFileName");

            var directory = Path.GetDirectoryName(solutionFileName.FullPath);

            if (null == directory)
                throw new FormatException(string.Format("Path.GetDirectoryName returned null for solution file [{0}]", solutionFileName));

            _log.InfoFormat("Reading Projects from Solution File [{0}]", solutionFileName.FullPath);

            foreach (Match match in ProjectLinePattern.Matches(_fileReader.ReadAllText(solutionFileName)))
            {
                if (!match.Success) 
                    continue;

                string typeGuid = match.Groups["TypeGuid"].Value;
                string title = match.Groups["Title"].Value;
                string location = match.Groups["Location"].Value;
                    
                switch (typeGuid.ToUpperInvariant())
                {
                    case "{2150E333-8FDC-42A3-9474-1A3956D46DE8}": // Solution Folder
                        // ignore folders
                        break;
                    case "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}": // C# project
                        yield return new SolutionFileProjectReference
                        {
                            ProjectFileName = new FilePath(directory, location),
                            Title = title
                        };

                        break;
                    default:
                        _log.InfoFormat("Project {0} has unsupported type {1}", location, typeGuid);
                        break;
                }
            }
        }
    }
}
