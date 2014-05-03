//----------------------------------------------------------------------- 
// <copyright file="CodeGenerationError.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, November 10, 2013 12:26:14 AM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure
{
    public class CodeGenerationError
    {
        public enum SeverityOptions
        {
            Message,
            Warning,
            Error
        }

        public CodeGenerationError(string message = "", SeverityOptions severity = SeverityOptions.Warning, int line = 0, int column = 0)
        {
            Message = message;
            Line = (uint)line;
            Column = (uint)column;
            Severity = severity;
        }
        
        public SeverityOptions Severity { get; set; }
        public uint Level { get; set; }
        public string Message { get; set; }
        public uint Line { get; set; }
        public uint Column { get; set; }
        public Exception Exception { get; set; }
    }
}
