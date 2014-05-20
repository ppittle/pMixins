//----------------------------------------------------------------------- 
// <copyright file="FilePathTests.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 20, 2014 10:22:53 AM</date> 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.UnitTests
{
    [TestFixture]
    public class FilePathTest
    {
        [Test]
        public void FilePathsWithSameCasingAreEqual()
        {
            const string fullPath = @"c:\temp\file.txt";

            new FilePath(fullPath).Equals(new FilePath(fullPath)).ShouldBeTrue();

            new FilePath(fullPath).ShouldEqual(new FilePath(fullPath));

            new FilePath(fullPath).Equals(new FilePath(fullPath), new FilePath(fullPath)).ShouldBeTrue();
        }

        [Test]
        public void FilePathsWithDifferentCasingAreEqual()
        {
            const string fullPath1 = @"c:\temp\file.txt";

            const string fullPath2 = @"C:\teMP\fiLe.tXt";

            new FilePath(fullPath1).Equals(new FilePath(fullPath2)).ShouldBeTrue();

            new FilePath(fullPath1).ShouldEqual(new FilePath(fullPath2));

            new FilePath(fullPath1).Equals(new FilePath(fullPath1), new FilePath(fullPath2)).ShouldBeTrue();
        }

        [Test]
        public void CanUseFilePathsInADictionary()
        {
            const string fullPath1 = @"c:\temp\file.txt";

            const string fullPath2 = @"C:\teMP\fiLe.tXt";

            const string noiseFullPath = @"D:\Misc\Folder\File.ext";

            var dict = new Dictionary<FilePath, object>()
            {
                {new FilePath(fullPath1), new object()},
                {new FilePath(noiseFullPath), new object()}
            };

            dict.ContainsKey(new FilePath(fullPath1)).ShouldBeTrue();

            dict.ContainsKey(new FilePath(fullPath1)).ShouldBeTrue();

            dict.ContainsKey(new FilePath(fullPath2)).ShouldBeTrue();

           

            try
            {
                dict.Add(new FilePath(fullPath2), new object());
                Assert.Fail("Expected Key Exception");
            }
            catch (ArgumentException e) { }
            
        }
    }
}
