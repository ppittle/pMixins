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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio
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

        public pMixinsCodeGeneratorResponseFileWriter(IFileWrapper fileWrapper, IFileReader fileReader)
        {
            _fileWrapper = fileWrapper;
            _fileReader = fileReader;
        }

        public void WriteCodeGeneratorResponse(CodeGeneratorResponse response)
        {
            string filePath = "<not defined>";

            try
            {
                if (null == response.CodeGeneratorContext)
                    return;

                filePath =
                    Path.Combine(
                        Path.GetDirectoryName(response.CodeGeneratorContext.Source.FileName) ?? "",
                        Path.GetFileNameWithoutExtension(response.CodeGeneratorContext.Source.FileName) ?? "")
                    + Constants.PMixinFileExtension;

                _log.InfoFormat("Updating [{0}]", filePath);

                if (_fileWrapper.Exists(filePath))
                {
                    _log.DebugFormat("Deleting file [{0}]", filePath);
                    _fileWrapper.Delete(filePath);
                }

                _fileWrapper.WriteAllText(filePath, response.GeneratedCodeSyntaxTree.GetText());

                _fileReader.EvictFromCache(filePath);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format("Exception writing Generated Code to [{0}]: {1}",
                        filePath,
                        e.Message), e);
            }
        }
        
        
    }
}
