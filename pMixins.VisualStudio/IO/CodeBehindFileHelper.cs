//----------------------------------------------------------------------- 
// <copyright file="CodeBehindFileHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 4:17:15 PM</date> 
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
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using EnvDTE;
using EnvDTE80;
using JetBrains.Annotations;
using log4net;
using VSLangProj;

namespace CopaceticSoftware.pMixins.VisualStudio.IO
{
    public interface ICodeBehindFileHelper
    {
        /// <summary>
        /// Returns the full path to a code behind file added for <paramref name="classFileName"/>.
        /// </summary>
        [CanBeNull]
        FilePath GetOrAddCodeBehindFile(FilePath classFileName);

        [CanBeNull]
        FilePath GetCodeBehindFile(FilePath classFileName);
    }

    public class CodeBehindFileHelper : ICodeBehindFileHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DTE2 _dte2;

        private readonly string _singleFileCodeGenerator;

        public CodeBehindFileHelper(DTE2 dte2, string singleFileCodeGenerator)
        {
            _dte2 = dte2;
            _singleFileCodeGenerator = singleFileCodeGenerator;
        }

        public FilePath GetOrAddCodeBehindFile(FilePath classFileName)
        {
            var target = LoadProjectItem(classFileName);

            if (null == target)
                return null;

            var previousCodeGenerator = target.Properties.Item("CustomTool").Value;

            try
            {
                target.Properties.Item("CustomTool").Value = _singleFileCodeGenerator;

                (target.Object as VSProjectItem).RunCustomTool();

                return GetCodeBehindFile(classFileName, target);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format(
                        "Failed running Code Generator [{0}] on File [{1}]: {2}",
                        _singleFileCodeGenerator,
                        classFileName,
                        e.Message), e);

                return null;
            }
            finally
            {
                //This line causes Visual Studio to exclude the code behind file
                //target.Properties.Item("CustomTool").Value = previousCodeGenerator;
            }
        }

        public FilePath GetCodeBehindFile(FilePath classFileName)
        {
            return GetCodeBehindFile(classFileName, LoadProjectItem(classFileName));
        }

        private FilePath GetCodeBehindFile(FilePath classFileName, ProjectItem target)
        {
            if (null == target)
                return null;

            var codeBehindFile = target.Properties.Item("CustomToolOutput").Value;

            if (null == codeBehindFile)
                return null;

            return
                new FilePath(
                    Path.GetDirectoryName(classFileName.FullPath),
                    codeBehindFile.ToString());
        }

        private ProjectItem LoadProjectItem(FilePath classFileName)
        {
            var target = _dte2.Solution.FindProjectItem(classFileName.FullPath);

            if (null == target)
            {
                _log.ErrorFormat("Failed to find Project Item [{0}]", classFileName);
                return null;
            }

            return target;
        }
    }
}
