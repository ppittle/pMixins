//----------------------------------------------------------------------- 
// <copyright file="IVisualStudioWriter.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 3:15:31 PM</date> 
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
    /// <summary>
    /// Proxies communication to the Visual Studio Output and Error List
    /// window.
    /// </summary>
    public interface IVisualStudioWriter : IDisposable
    {
        void GeneratorError(string message, uint line, uint column);
        void GeneratorWarning(string message, uint line, uint column);

        void GeneratorMessage(string message, uint line, uint column);
        void OutputString(string s);

        void WriteToStatusBar(string s);

        void SetStatusProgress(uint cookie, int progress, string label, uint complete, uint total);

        void ClearStatusBar();
    }

    
}
