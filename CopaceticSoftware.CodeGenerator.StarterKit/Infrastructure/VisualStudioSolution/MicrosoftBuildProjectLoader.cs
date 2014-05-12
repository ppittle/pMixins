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
using log4net;
using Microsoft.Build.Evaluation;
using Project = Microsoft.Build.Evaluation.Project;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface IMicrosoftBuildProjectLoader
    {
        Project LoadMicrosoftBuildProject(string projectFileName);
    }

    public class MicrosoftBuildProjectLoader : IMicrosoftBuildProjectLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly object _lock = new object();

        public Project LoadMicrosoftBuildProject(string projectFileName)
        {
            lock (_lock)
            {
                var loadedProjects =
                    ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFileName)
                        //Create a copy incase the collection is modified externally during iteration
                        .ToArray();

                return loadedProjects.Any()
                    ? loadedProjects.First()
                    : new Project(projectFileName);
            }
        }
    }
}
