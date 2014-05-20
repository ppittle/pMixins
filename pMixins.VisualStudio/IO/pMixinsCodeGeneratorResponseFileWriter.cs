//----------------------------------------------------------------------- 
// <copyright file="pMixinsCodeGeneratorResponseFileWriter.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 3:28:37 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio.IO
{
    public interface IpMixinsCodeGeneratorResponseFileWriter
    {
        void WriteCodeGeneratorResponse(CodeGeneratorResponse response);
        void ClearCodeBehindForSourceFile(FilePath sourceFullPath);
    }

    public class pMixinsCodeGeneratorResponseFileWriter : IpMixinsCodeGeneratorResponseFileWriter
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFileWrapper _fileWrapper;
        private readonly IFileReader _fileReader;
        private readonly ICodeBehindFileHelper _codeBehindFileHelper;
        private readonly IVisualStudioOpenDocumentManager _visualStudioOpenDocumentManager;

        public pMixinsCodeGeneratorResponseFileWriter(IFileWrapper fileWrapper, IFileReader fileReader, ICodeBehindFileHelper codeBehindFileHelper, IVisualStudioOpenDocumentManager visualStudioOpenDocumentManager)
        {
            _fileWrapper = fileWrapper;
            _fileReader = fileReader;
            _codeBehindFileHelper = codeBehindFileHelper;
            _visualStudioOpenDocumentManager = visualStudioOpenDocumentManager;
        }

        public void WriteCodeGeneratorResponse(CodeGeneratorResponse response)
        {
            try
            {
                if (null == response.CodeGeneratorContext)
                    return;

                var codeBehindFileName = _codeBehindFileHelper.GetOrAddCodeBehindFile(
                    response.CodeGeneratorContext.Source.FileName);

                if (codeBehindFileName.IsNullOrEmpty())
                {
                    _log.Error("Code Behind File Helper returned a null or empty CodeBehindFileName. Can not write file.");
                    return;
                }

                _log.InfoFormat("Updating [{0}]", codeBehindFileName);

                if (_fileWrapper.Exists(codeBehindFileName))
                {
                    _log.DebugFormat("Deleting file [{0}]", codeBehindFileName);
                    _fileWrapper.Delete(codeBehindFileName);
                }

                var codeBehindFileSource = response.GeneratedCodeSyntaxTree.GetText();

                if (string.IsNullOrEmpty(codeBehindFileSource))
                    _log.WarnFormat("Writing Empty Code Behind File for [{0}]", codeBehindFileName);

                _fileWrapper.WriteAllText(codeBehindFileName, codeBehindFileSource);

                _fileReader.EvictFromCache(codeBehindFileName);

                var openWindow = _visualStudioOpenDocumentManager.GetOpenDocument(codeBehindFileName);

                if (null != openWindow)
                    openWindow.WriteText(codeBehindFileSource);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format("Exception writing Generated Code for Source Class [{0}]: {1}",
                        response.CodeGeneratorContext.Source.FileName,
                        e.Message), e);
            }
        }

        public void ClearCodeBehindForSourceFile(FilePath sourceFullPath)
        {
            try
            {
                var codeBehind = _codeBehindFileHelper.GetCodeBehindFile(sourceFullPath);

                if (null == codeBehind)
                {
                    _log.InfoFormat("No Code Behind found to clear for [{0}]", sourceFullPath);
                    return;
                }

                if (_fileWrapper.Exists(codeBehind))
                {
                    _log.DebugFormat("Deleting file [{0}]", codeBehind);
                    _fileWrapper.Delete(codeBehind);
                }

                _fileWrapper.WriteAllText(codeBehind, string.Empty);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format("Exception clearing Code Behind for Source Class [{0}]: {1}",
                        sourceFullPath,
                        e.Message), e);
            }
        }
    }
}
