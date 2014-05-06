//----------------------------------------------------------------------- 
// <copyright file="SolutionExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 11:47:55 PM</date> 
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
using System.Linq;
using System.Reflection;
using System.Text;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.Common.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;
using JetBrains.Annotations;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    
    public static class SolutionExtensions
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [CanBeNull]
        public static CSharpFile AddOrUpdateProjectItemFile(this Solution solution, RawSourceFile rawSourceFile)
        {
            Ensure.ArgumentNotNull(solution, "solution");
            
            try
            {
                var project = solution.Projects.FirstOrDefault(
                    p => p.FileName.Equals(rawSourceFile.ProjectFileName, StringComparison.InvariantCultureIgnoreCase));

                if (null == project)
                {
                    _log.ErrorFormat("Failed AddOrUpdateProjectItemFile [{0}] because could not find Project [{1}]",
                        rawSourceFile.FileName,
                        rawSourceFile.ProjectFileName);

                    return null;
                }

                var csharpFile = new CSharpFile(project, rawSourceFile.FileName, rawSourceFile.FileContents);

                project.AddOrUpdateCSharpFile(csharpFile);

                solution.RecreateCompilations();

                return csharpFile;
            }
            catch (Exception e)
            {
                _log.Error("Excpetion in AddOrUpdateCodeGeneratorFileSource: " + e.Message, e);
                throw;
            }
        }

        [CanBeNull]
        public static CSharpFile FindFileForIType(this Solution solution, IType type)
        {
            return
                solution.AllFiles.FirstOrDefault(
                    f => f.UnresolvedTypeSystemForFile.TopLevelTypeDefinitions.Any(td => td.FullName == type.FullName));
        }
    }
}
