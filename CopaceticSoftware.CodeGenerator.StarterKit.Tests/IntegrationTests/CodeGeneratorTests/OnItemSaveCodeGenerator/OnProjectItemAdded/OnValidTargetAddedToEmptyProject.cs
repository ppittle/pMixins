//----------------------------------------------------------------------- 
// <copyright file="TargetClassIsAddedToAnEmptyProject.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 12:16:14 PM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnProjectItemAdded
{
    public abstract class OnValidTargetAddedToEmptyProject : OnItemSaveCodeGeneratorTestBase
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
    public class OnValidTargetWithMixinInSameClassAddedToEmptyProject : OnValidTargetAddedToEmptyProject
    {
        private MockSourceFile _sourceFileWithTarget;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSameClass();

            //Save source file - it will be added after solution openend
            _sourceFileWithTarget = 
                _MockSolution.RemoveFile(s => s.AllMockSourceFiles.First());
        }

        public override void MainSetup()
        {
            base.MainSetup();

            this.AddMockSourceFile(s => s.Projects[0], _sourceFileWithTarget);
        }

        //[Test] - Use Tests from Base Class - OnItemSaveWithValidTargetFile
    }

    [TestFixture]
    public class OnValidTargetFileWithMixinInSameProjectAddedToProject : OnValidTargetAddedToEmptyProject
    {
        private MockSourceFile _sourceFileWithTarget;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSeparateClass();

            //Save source file - it will be added after solution openend
            _sourceFileWithTarget =
                _MockSolution.RemoveFile(s => s.AllMockSourceFiles.First(f => f.ContainsPMixinAttribute));
        }

        public override void MainSetup()
        {
            base.MainSetup();

            this.AddMockSourceFile(s => s.Projects[0], _sourceFileWithTarget);
        }

        //[Test] - Use Tests from Base Class - OnItemSaveWithValidTargetFile
    }

    [TestFixture]
    public class OnValidTargetFileWithTwoTargetsAndMixinInSeparateClassAddedToProject : OnValidTargetAddedToEmptyProject
    {
        private MockSourceFile[] _sourceFilesWithTargets;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSameClass();

            //Save source file - it will be added after solution openend
            _sourceFilesWithTargets = 
                _MockSolution.RemoveFiles(s => s.AllMockSourceFiles.Where(f => f.ContainsPMixinAttribute));
        }

        public override void MainSetup()
        {
            base.MainSetup();

            _sourceFilesWithTargets
                .Map(f => this.AddMockSourceFile(s => s.Projects[0], f));
        }

        //[Test] - Use Tests from Base Class - OnItemSaveWithValidTargetFile
    }

    public class OnValidTargetFileWithTwoMixinsAddedToProject : OnValidTargetAddedToEmptyProject
    {
        private MockSourceFile _sourceFileWithTarget;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetWithTwoMixins();

            //Save source file - it will be added after solution openend
            _sourceFileWithTarget =
                _MockSolution.RemoveFile(s => s.AllMockSourceFiles.First(f => f.ContainsPMixinAttribute));
        }

        public override void MainSetup()
        {
            base.MainSetup();

            this.AddMockSourceFile(s => s.Projects[0], _sourceFileWithTarget);
        }

        protected override void CanExecuteMixedInMethodImpl()
        {
            var _sourceFile = _MockSolution.AllMockSourceFiles.First(f => f.ContainsPMixinAttribute);

            _sourceFile.AssertCompilesAndCanExecuteMethod(_MockSolution, "TestMethod1");

            _sourceFile.AssertCompilesAndCanExecuteMethod(_MockSolution, "TestMethod2");
        }

        //[Test] - Use Tests from Base Class - OnItemSaveWithValidTargetFile
    }

    public class OnValidTargetFileWithMixinInSeparateProjectAddedToProject : OnValidTargetAddedToEmptyProject
    {
        private MockSourceFile _sourceFileWithTarget;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSeparateProjects();

            //Save source file - it will be added after solution openend
            _sourceFileWithTarget =
                _MockSolution.RemoveFile(s => s.AllMockSourceFiles.First(f => f.ContainsPMixinAttribute));
        }

        public override void MainSetup()
        {
            base.MainSetup();

            this.AddMockSourceFile(s => s.Projects[0], _sourceFileWithTarget);
        }

        //[Test] - Use Tests from Base Class - OnItemSaveWithValidTargetFile
    }
}
