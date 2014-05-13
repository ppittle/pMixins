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
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using log4net;
using Microsoft.Build.Evaluation;

namespace CopaceticSoftware.pMixins.VisualStudio.IO
{
    public interface IpMixinsCodeGeneratorResponseFileWriter
    {
        void WriteCodeGeneratorResponse(CodeGeneratorResponse response);
    }

    public class pMixinsCodeGeneratorResponseFileWriter : IpMixinsCodeGeneratorResponseFileWriter
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFileWrapper _fileWrapper;
        private readonly IFileReader _fileReader;
        private readonly ICodeBehindFileHelper _codeBehindFileHelper;

        public pMixinsCodeGeneratorResponseFileWriter(IFileWrapper fileWrapper, IFileReader fileReader, ICodeBehindFileHelper codeBehindFileHelper)
        {
            _fileWrapper = fileWrapper;
            _fileReader = fileReader;
            _codeBehindFileHelper = codeBehindFileHelper;
        }

        public void WriteCodeGeneratorResponse(CodeGeneratorResponse response)
        {
            try
            {
                if (null == response.CodeGeneratorContext)
                    return;

                var codeBehindFileName = _codeBehindFileHelper.GetOrAddCodeBehindFile(
                    response.CodeGeneratorContext.Source.FileName);

                if (string.IsNullOrEmpty(codeBehindFileName))
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

                _fileWrapper.WriteAllText(codeBehindFileName, response.GeneratedCodeSyntaxTree.GetText());

                _fileReader.EvictFromCache(codeBehindFileName);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format("Exception writing Generated Code for Source Class [{0}]: {1}",
                        response.CodeGeneratorContext.Source.FileName,
                        e.Message), e);
            }
        }
    }
}
