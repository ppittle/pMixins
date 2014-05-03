//----------------------------------------------------------------------- 
// <copyright file="SolutionExtender.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, January 13, 2014 4:39:58 PM</date> 
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
using System.IO;
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD.NRefactory;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution.OLD
{
    public interface ISolutionExtender
    {
        event EventHandler<NRefactory.CSharpProject.AssemblyReferencesResolved> OnProjectAssemblyReferencesResolved;

        NRefactory.Solution Solution { get; }
        NRefactory.CSharpFile AddOrUpdateProjectItemFile(string projectFilePath, string codeRelativeFilePath, string sourceCode);
        NRefactory.CSharpProject AddOrUpdateProject(string projectFilePath);
        void RemoveCSharpFileFromProject(NRefactory.CSharpProject project, string codeFilePath);
        void AddFileToProject(string projectFilePath, string codeFilePath);
        NRefactory.CSharpProject GetProjectByFilePath(string projectFilePath);
        void RemoveProject(NRefactory.CSharpProject project);
    }

    /// <summary>
    /// Adds addational 'meta' functionality to a wrapped <see cref="Solution"/>
    /// </summary>
    public class SolutionExtender : ISolutionExtender
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public event EventHandler<NRefactory.CSharpProject.AssemblyReferencesResolved> OnProjectAssemblyReferencesResolved;

        private NRefactory.Solution _solution;
        public NRefactory.Solution Solution
        {
            get { return _solution; }

            private set
            {
                if (null != value)
                    foreach (var p in value.Projects)
                        p.OnAssemblyReferencesResolved += (sender, resolved) =>
                                                          {
                                                              if (null != OnProjectAssemblyReferencesResolved)
                                                                  OnProjectAssemblyReferencesResolved(sender, resolved);
                                                          };

                _solution = value;
            }
        }


        public SolutionExtender(NRefactory.Solution solution)
        {
            Ensure.ArgumentNotNull(solution, "solution");

            Solution = solution;
        }


        public NRefactory.CSharpFile AddOrUpdateProjectItemFile(string projectFilePath, string codeRelativeFilePath, string sourceCode)
        {
            var projectAbsoluteFilePath = PathExt.MakePathAbsolute(Solution.FullName, projectFilePath);
            var codeAbsoluteFilePath = PathExt.MakePathAbsolute(projectAbsoluteFilePath, codeRelativeFilePath);

            try
            {
                //Add Or Get CSharpProject in Solution by Project Name
                var project = AddOrUpdateProject(projectAbsoluteFilePath);

                //If CSharpFile exists (by fileName) in CSharpProject remove it 
                RemoveCSharpFileFromProject(project, codeAbsoluteFilePath);

                //Create a new CSharpFile
                var newFile = new NRefactory.CSharpFile(project, codeAbsoluteFilePath, sourceCode);

                //Add the file to the Project
                project.AddCSharpFile(newFile);

                //Recreate the compilation so the Type system is up to date.
                Solution.RecreateCompilations();

                return newFile;

            }
            catch (Exception e)
            {
                _log.Error("Excpetion in AddOrUpdateCodeGeneratorFileSource: " + e.Message, e);
                throw;
            }
        }

        public NRefactory.CSharpProject AddOrUpdateProject(string projectFilePath)
        {
            _log.InfoFormat("Looking for Project [{0}]", projectFilePath);

            var project = GetProjectByFilePath(projectFilePath);

            if (null != project)
            {
                _log.Info("Project has already been parsed.");

                Solution.Projects.Remove(project);

                var newProject = new NRefactory.CSharpProject(Solution, project.Title, project.FileName);

                Solution.Projects.Add(newProject);

                Solution.RecreateCompilations();

                return newProject;
            }

            _log.Info("Project has not been parsed.  Adding it now.");
            //Reparse the whole solution - TODO: can probably optomize this.
            Solution = new NRefactory.Solution(Solution.FullName);

            project = GetProjectByFilePath(projectFilePath);

            if (null == project)
                throw new Exception(string.Format(
                    "Failed to load project [{0}].  Does it exist in Solution [{1}]",
                    projectFilePath, Solution.FileName));

            return project;
        }

        public void RemoveCSharpFileFromProject(NRefactory.CSharpProject project, string codeAbsoluteFilePath)
        {
            Ensure.ArgumentNotNull(project, "project");

            for (var i = 0; i < project.Files.Count; i++)
                if (PathExt.PathsAreEqual(project.Files[i].FileName, codeAbsoluteFilePath))
                    project.Files.RemoveAt(i);
        }

        public void AddFileToProject(string projectFilePath, string codeFilePath)
        {
            var project = GetProjectByFilePath(projectFilePath);

            if (null == project)
                throw new Exception(string.Format(Strings.ExceptionCouldNotFindProjectWithFullName, projectFilePath));

            project.AddCSharpFile(new NRefactory.CSharpFile(project, codeFilePath));
        }

        public NRefactory.CSharpProject GetProjectByFilePath(string projectFilePath)
        {
            projectFilePath = Path.GetFullPath(projectFilePath);

            return
                Solution.Projects.FirstOrDefault(x => PathExt.PathsAreEqual(x.FileName, projectFilePath));
        }

        public void RemoveProject(NRefactory.CSharpProject project)
        {
            if (null == project)
                return;

            throw new NotImplementedException();
        }
    }
}
