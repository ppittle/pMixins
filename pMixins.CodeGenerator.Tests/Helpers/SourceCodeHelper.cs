//----------------------------------------------------------------------- 
// <copyright file="SourceCodeHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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
using System.Text;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.Helpers
{
    public static class SourceCodeHelper
    {
        public static string CleanCode(string code)
        {
            return
                RemoveHeaderComment(code)
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "");
        }

        public static string RemoveHeaderComment(string source)
        {

            var sb = new StringBuilder();

            using (var sr = new StringReader(source))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!line.StartsWith("//"))
                        sb.Append(line);
                }
            }

            return sb.ToString();
        }
    }
}
