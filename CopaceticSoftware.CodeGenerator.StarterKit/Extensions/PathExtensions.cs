//----------------------------------------------------------------------- 
// <copyright file="PathExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 1:47:10 AM</date> 
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
using System.Runtime.InteropServices;
using System.Text;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    //http://pinvoke.net/default.aspx/shlwapi.PathRelativePathTo
    public static class PathExtensions
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] FileAttributes dwAttrFrom,
             [In] string pszTo,
             [In] FileAttributes dwAttrTo
        );

        public static string PathRelativePathTo(string path1, string path2)
        {
            var sb = new StringBuilder(260);

            var path2FileAttribute =
                string.IsNullOrEmpty(Path.GetExtension(path2))
                    ? FileAttributes.Directory
                    : FileAttributes.Normal;

            return 
                PathRelativePathTo(
                    sb,
                    Path.GetDirectoryName(path1),
                    FileAttributes.Directory,
                    path2,
                    path2FileAttribute) 
                
                ? sb.ToString() 
                : string.Empty;
        }
    }
}
