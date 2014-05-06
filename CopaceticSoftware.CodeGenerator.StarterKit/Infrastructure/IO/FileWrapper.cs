//----------------------------------------------------------------------- 
// <copyright file="FileWrapper.cs" company="Copacetic Software"> 
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

using System.IO;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO
{
    public interface IFileWrapper
    {
        string ReadAllText(string filename);
        bool Exists(string filename);
        void Delete(string filename);
        void WriteAllText(string filename, string s);
    }

    public class FileWrapper : IFileWrapper
    {
        public string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }

        public bool Exists(string filename)
        {
            return File.Exists(filename);
        }

        public void Delete(string filename)
        {
            File.Delete(filename);
        }

        public void WriteAllText(string filename, string s)
        {
            File.WriteAllText(filename, s);
        }

    }
}
