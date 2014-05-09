//----------------------------------------------------------------------- 
// <copyright file="VisualStudioOpenDocumentReader.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, May 5, 2014 3:58:03 PM</date> 
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
using System.Diagnostics;
using System.Reflection;
using EnvDTE;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO
{
    public interface IVisualStudioOpenDocumentReader
    {
        string GetDocumentText();
    }

    /// <summary>
    /// http://us.generation-nt.com/answer/read-entire-contents-envdte-document-help-21147682.html
    /// </summary>
    [DebuggerDisplay("{_classFileName}")]
    public class VisualStudioOpenDocumentReader : IVisualStudioOpenDocumentReader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly TextDocument _textDocument;
        private readonly string _classFileName;

        public VisualStudioOpenDocumentReader(Document document)
        {
            try
            {
                //http://msdn.microsoft.com/en-us/library/ms228776.aspx
                _textDocument = (EnvDTE.TextDocument) (document.Object("TextDocument"));
            }
            catch (Exception e)
            {
                _log.Warn("Failed to create a TextDocument from Document: " + e.Message, e);
            }
           
            _classFileName = document.FullName;
        }

        public string GetDocumentText()
        {
            if (null == _textDocument)
                return string.Empty;

            try
            {
                var editPoint = _textDocument.StartPoint.CreateEditPoint();

                return editPoint.GetText(_textDocument.EndPoint);
            }
            catch (Exception e)
            {
                _log.Error(
                    string.Format("Exception reading TextDocument for file [{0}]: {1}",
                        _classFileName, e.Message), e);

                return string.Empty;
            }
        }
    }
}
