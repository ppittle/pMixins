﻿//----------------------------------------------------------------------- 
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
using System.IO;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.Build.Evaluation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public class MicrosoftBuildProject
    {
        public readonly string FileName;
        public readonly string AssemblyName;
        public readonly bool AllowUnsafeBlocks;
        public readonly bool CheckForOverflowUnderflow;
        public readonly IEnumerable<string> DefineConstants;
        public readonly IEnumerable<string> CompiledFileNames;
        public readonly IEnumerable<IUnresolvedAssembly> ReferencedAssemblies; 

        public MicrosoftBuildProject(
            IMicrosoftBuildProjectAssemblyReferenceResolver assemblyReferenceResolver,
            string projectFileName)
        {
            var msBuildProject = GetMSBuildProject(projectFileName);
            
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

            ReferencedAssemblies = assemblyReferenceResolver.ResolveAssemblyReferences(msBuildProject);}

        private Project GetMSBuildProject(string projectFileName)
        {
            var loadedProjects = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(projectFileName);

            return loadedProjects.Any()
                ? loadedProjects.First()
                : new Project(projectFileName);
        }
    }
}