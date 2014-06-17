//----------------------------------------------------------------------- 
// <copyright file="MicrosoftBuildProjectLoader.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 9:01:51 PM</date> 
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
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using log4net;
using Microsoft.Build.Evaluation;
using Project = Microsoft.Build.Evaluation.Project;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface IMicrosoftBuildProjectLoader
    {
        Project LoadMicrosoftBuildProject(FilePath projectFileName);
    }

    public class MicrosoftBuildProjectLoader : IMicrosoftBuildProjectLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object _lock = new object();

        public MicrosoftBuildProjectLoader(IVisualStudioEventProxy visualStudioEventProxy)
        {
            //Make sure there aren't any latent project references
            visualStudioEventProxy.OnProjectRemoved += (sender, args) =>
                RemoveAllReferencesToProject(args.ProjectFullPath);
        }

        private void RemoveAllReferencesToProject(FilePath projectFileName)
        {
            lock (_lock)
                try
                {
                    _log.InfoFormat("OnProjectAdded: Clear ProjectCollection for [{0}]", projectFileName);

                    ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFileName.FullPath)
                        .Map(p => ProjectCollection.GlobalProjectCollection.UnloadProject(p));
                }
                catch (Exception e)
                {
                    _log.Error(string.Format("Exception trying to unload Project [{0}] : {1}",
                        projectFileName,
                        e.Message));
                }
        }

        public Project LoadMicrosoftBuildProject(FilePath projectFileName)
        {
            lock (_lock)
            {
                try
                {
                    var loadedProjects =
                        ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFileName.FullPath)
                            //Create a copy incase the collection is modified externally during iteration
                            .ToArray();

                    return loadedProjects.Any()
                        ? loadedProjects.First()
                        : new Project(projectFileName.FullPath);
                }
                catch (Exception e)
                {
                    _log.Error("Exception Loading Microsoft Build Project for [" + projectFileName.FullPath + "]", e);
                    throw;
                }
            }
        }
    }
}
