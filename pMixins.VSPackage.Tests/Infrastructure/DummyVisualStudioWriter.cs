//----------------------------------------------------------------------- 
// <copyright file="DummyVisualStudioWriter.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 4, 2014 3:41:38 PM</date> 
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

using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using log4net;

namespace CopaceticSoftware.pMixins.VSPackage.Tests.Infrastructure
{
    public class DummyVisualStudioWriter : IVisualStudioWriter
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void GeneratorError(string message, uint line, uint column)
        {
            _log.ErrorFormat("[{0}] [{1}] [{2}]", line, column, message);
        }

        public void GeneratorWarning(string message, uint line, uint column)
        {
            _log.WarnFormat("[{0}] [{1}] [{2}]", line, column, message);
        }

        public void GeneratorMessage(string message, uint line, uint column)
        {
            _log.InfoFormat("[{0}] [{1}] [{2}]", line, column, message);
        }

        public void OutputString(string s)
        {
            _log.InfoFormat("Outputpane: {0}", s);
        }

        public void WriteToStatusBar(string s)
        {
            _log.InfoFormat("Status Bar: {0}", s);
        }

        public void SetStatusProgress(uint cookie, int progress, string label, uint complete, uint total)
        {
            
        }

        public void ClearStatusBar()
        {
         
        }
    }
}
