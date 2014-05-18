//----------------------------------------------------------------------- 
// <copyright file="MicrosoftBuildProject.cs" company="Copacetic Software"> 
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using ICSharpCode.NRefactory.TypeSystem;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public class MicrosoftBuildProject
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly FilePath FileName;
        public readonly string AssemblyName;
        public readonly bool AllowUnsafeBlocks;
        public readonly bool CheckForOverflowUnderflow;
        public readonly IEnumerable<string> DefineConstants;
        public readonly IEnumerable<string> CompiledFileNames;
        public readonly IAssemblyReference[] ReferencedAssemblies; 

        public MicrosoftBuildProject(
            IMicrosoftBuildProjectLoader microsoftBuildProjectLoader,
            IMicrosoftBuildProjectAssemblyReferenceResolver assemblyReferenceResolver,
            FilePath projectFileName)
        {
            var sw = Stopwatch.StartNew();

            FileName = projectFileName;

            var msBuildProject = microsoftBuildProjectLoader.LoadMicrosoftBuildProject(projectFileName);
            
            AssemblyName = msBuildProject.GetPropertyValue("AssemblyName");
            AllowUnsafeBlocks = msBuildProject.GetBoolProperty("AllowUnsafeBlocks") ?? false;
            CheckForOverflowUnderflow = msBuildProject.GetBoolProperty("CheckForOverflowUnderflow") ?? false;

            DefineConstants =
                msBuildProject.GetPropertyValue("DefineConstants")
                    .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim());

            CompiledFileNames =
                msBuildProject.GetItems("Compile")
                    .Select(i => Path.Combine(msBuildProject.DirectoryPath, i.EvaluatedInclude));

            ReferencedAssemblies =
                assemblyReferenceResolver.ResolveReferences(msBuildProject);
                

            _log.DebugFormat("Project [{0}] built in [{1}] ms", Path.GetFileName(FileName.FullPath), sw.ElapsedMilliseconds);
        }
    }
}
