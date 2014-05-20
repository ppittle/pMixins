//----------------------------------------------------------------------- 
// <copyright file="FilePath.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 18, 2014 7:15:53 PM</date> 
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO
{
    /// <summary>
    /// Helper class for dealing with File Paths
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/23757210/c-sharp-dictionary-file-path-custom-equalitycomparer
    /// </remarks>
    [DebuggerDisplay("{FullPath}")]
    public class FilePath : EqualityComparer<FilePath>, IComparable<FilePath>, IEquatable<FilePath>
    {
        public string FullPath { get; private set; }

        public string Extension { get; private set; }

        public FilePath(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath))
            {
                FullPath = string.Empty;
                Extension = string.Empty;

                return;
            }

            FullPath = Path.GetFullPath(fullpath);
            Extension = Path.GetExtension(FullPath).ToLower();
        }

        public FilePath(string directory, string relativePath)
            : this(Path.Combine(directory ?? "", relativePath ?? ""))
        {
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(FullPath);
        }

        public bool Equals(FilePath other)
        {
            return Equals(this, other);
        }

        public int CompareTo(FilePath other)
        {
            return Compare(this, other);
        }

        public override string ToString()
        {
            return FullPath;
        }

        public override bool Equals(FilePath x, FilePath y)
        {
            if (x == null || null == y)
                return false;

            return 0 == Compare(x, y);
        }

        public override int GetHashCode(FilePath obj)
        {
            return obj.FullPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as FilePath);
        }

        public override int GetHashCode()
        {
            return FullPath.ToLower().GetHashCode();
        }
       
        public int Compare(FilePath x, FilePath y)
        {
            if (null == x && null == y)
                throw new Exception("Both x and y are null.  Not sure what to do here.");

            if (null == x)
                return -1;
            if (null == y)
                return 1;

            if (x.FullPath.Equals(y.FullPath, StringComparison.InvariantCultureIgnoreCase))
                return 0;

            if (x.FullPath.Length <= y.FullPath.Length)
                return -1;

            return 1;
        }
    }


    public static class FilePathExtensions
    {
        public static bool IsNullOrEmpty(this FilePath f)
        {
            return null == f || string.IsNullOrEmpty(f.FullPath);
        }
    }
}
