//----------------------------------------------------------------------- 
// <copyright file="PathExtensionsTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, July 22, 2014 10:48:31 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.UnitTests
{
    /// <summary>
    /// Tests for <see cref="PathExtensions"/>
    /// </summary>
    [TestFixture]
    public class PathExtensionsTest
    {
        [Test]
        public void SourceFileRelativeToSolutionFile()
        {
            var solutionFile = @"d:\someFolder\solution\solution.sln";
            var classFile = @"D:\SomeFolder\Solution\Project\Folder\class.cs";

            var relativeClassFile =
                PathExtensions.MakeRelativePath(
                    Path.GetDirectoryName(solutionFile),
                    classFile);

            relativeClassFile.ToLower().ShouldEqual("solution\\project\\folder\\class.cs");
        }

        [Test]
        public void SourceFileOutsideOfSolution()
        {
            var solutionFile = @"d:\someFolder\solution\solution.sln";
            var classFile = @"c:\external\class.cs";

            var relativeClassFile =
                PathExtensions.MakeRelativePath(
                    Path.GetDirectoryName(solutionFile),
                    classFile);

            relativeClassFile.ToLower().ShouldEqual(classFile);
        }
    }
}
