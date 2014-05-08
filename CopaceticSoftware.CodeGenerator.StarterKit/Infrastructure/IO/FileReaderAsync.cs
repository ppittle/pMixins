//----------------------------------------------------------------------- 
// <copyright file="FileReaderAsync.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 1:28:27 PM</date> 
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
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO
{
    public class FileReaderAsync
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Task _readFileTask;

        private Exception _fileReadException = null;

        private string _fileContents;

        private readonly string _fileName;

        private readonly IVisualStudioOpenDocumentManager _openDocumentManager;

        public FileReaderAsync(IVisualStudioOpenDocumentManager openDocumentManager, IFileWrapper fileWrapper, string filename)
        {
            _fileName = filename;
            _openDocumentManager = openDocumentManager;

            _readFileTask = new TaskFactory().StartNew(() =>
            {
                try
                {
                    _log.DebugFormat("Reading File [{0}]", filename);

                    _fileContents = fileWrapper.ReadAllText(filename);
                }
                catch (Exception e)
                {
                    _fileReadException = e;
                }
            });
        }

        
        public string FileContents
        {
            get
            {
                var openDocumentReader = _openDocumentManager.GetOpenDocument(_fileName);

                if (null != openDocumentReader)
                {
                    _log.InfoFormat("Document is open in editor [{0}]", _fileName);

                    return openDocumentReader.GetDocumentText();
                }

                _readFileTask.Wait();

                if (null != _fileReadException)
                    throw _fileReadException;

                return _fileContents;
            }
        }
    }
}
