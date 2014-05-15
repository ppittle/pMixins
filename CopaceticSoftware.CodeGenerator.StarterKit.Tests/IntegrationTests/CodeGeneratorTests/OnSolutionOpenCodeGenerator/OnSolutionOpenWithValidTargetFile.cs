//----------------------------------------------------------------------- 
// <copyright file="OnSolutionOpenWithValidTargetFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 14, 2014 11:13:18 PM</date> 
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

using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator
{
    public abstract class OnSolutionOpenWithValidTargetFile : OnSolutionOpenCodeGeneratorTestBase
    {
        protected virtual IEnumerable<MockSourceFile> GetTargetFiles
        {
            get { return _MockSolution.AllMockSourceFiles.Where(f => f.ContainsPMixinAttribute); }
        }

        [Test]
        public virtual void CodeBehindFilesAreGenerated()
        {
            GetTargetFiles.Map(
                f => this.AssertCodeBehindFileWasGenerated(f.FileName));
        }

        protected virtual void CanExecuteMixedInMethodImpl()
        {
            GetTargetFiles.Map(
               f => f.AssertCompilesAndCanExecuteMethod(_MockSolution));
        }

        [Test]
        public void CanExecuteMixedInMethod()
        {
            CanExecuteMixedInMethodImpl();
        }
    }

    [TestFixture]
    public class OnSolutionOpenWithValidTargetFileWithMixinInSameClassFile : OnSolutionOpenWithValidTargetFile
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSameClass();
        }
    }

    [TestFixture]
    public class OnSolutionOpenWithValidTargetFileWithMixinInSameProject : OnSolutionOpenWithValidTargetFile
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSeparateClass();
        }
    }

    [TestFixture]
    public class OnSolutionOpenWithValidTargetFileWithTwoTargetsAndMixinInSeparateClass : OnSolutionOpenWithValidTargetFile
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTwoTargetsAndMixinInSeparateClass();
        }
    }

    [TestFixture]
    public class OnSolutionOpenWithValidTargetFileWithTargetWithTwoMixins : OnSolutionOpenWithValidTargetFile
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetWithTwoMixins();
        }

        protected override void CanExecuteMixedInMethodImpl()
        {
            var _sourceFile = _MockSolution.AllMockSourceFiles.First(f => f.ContainsPMixinAttribute);

            _sourceFile.AssertCompilesAndCanExecuteMethod(_MockSolution, "TestMethod1");

            _sourceFile.AssertCompilesAndCanExecuteMethod(_MockSolution, "TestMethod2");
        }
    }

    [TestFixture]
    public class OnSolutionOpenWithValidTargetFileWithMixinInSeparateProject : OnSolutionOpenWithValidTargetFile
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSeparateProjects();
        }
    }

    
}
