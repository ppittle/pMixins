//----------------------------------------------------------------------- 
// <copyright file="OnItemSaveCodeGeneratorTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 10, 2014 8:00:47 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using Ninject;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator
{

    public abstract class OnSolutionOpenWithValidTargetFile : OnItemSaveCodeGeneratorTestBase
    {
        protected abstract MockSolution InitializeSolution();

        protected virtual IEnumerable<MockSourceFile> GetTargetFiles
        {
            get { return _MockSolution.AllMockSourceFiles.Where(f => f.ContainsPMixinAttribute); }
        }

        public override void MainSetup()
        {
            base.MainSetup();

            InitializeSolution();

            this.FireSolutionOpen();
        }

        [Test]
        public virtual void CodeBehindFilesAreGenerated()
        {
            GetTargetFiles.Map(
                f => this.AssertCodeBehindFileWasGenerated(f.FileName));
        }

        [Test]
        public virtual void CanExecuteMixedInMethod()
        {
            GetTargetFiles.Map(
                f => f.AssertCompilesAndCanExecuteMethod(
                    _MockSolution.Projects.FirstOrDefault(p => p.ContainsFile(f))));
        }
    }

    public class OnSolutionOpenWithNoTargetFile : OnItemSaveCodeGeneratorTestBase
    {
        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.InitializeWithNormalClassFile();

            this.FireSolutionOpen();
        }

        [Test]
        public void NoCodeBehindFileIsGenerated()
        {
            this.AssertCodeBehindFileWasNotGenerated();
        }
    }

    

    public abstract class OnItemSaveCodeGeneratorTestBase : MockSolutionTestBase
    {
        protected pMixinsOnItemSaveCodeGenerator _PMixinsOnItemSaveCodeGenerator;

        public IVisualStudioCodeGenerator _mockVisualStudioCodeGenerator;

        public VisualStudioCodeGenerator _actualVisualStudioCodeGenerator;

        public override void MainSetup()
        {
            base.MainSetup();

            _PMixinsOnItemSaveCodeGenerator = TestSpecificKernel.Get<pMixinsOnItemSaveCodeGenerator>();

            SetupCodeGeneratorMock();

            TestSpecificKernel.Rebind<IVisualStudioCodeGenerator>().ToMethod(x => _mockVisualStudioCodeGenerator);
        }

        private void SetupCodeGeneratorMock()
        {
            _actualVisualStudioCodeGenerator = TestSpecificKernel.Get<VisualStudioCodeGenerator>();

            _mockVisualStudioCodeGenerator = MockRepository.GenerateMock<IVisualStudioCodeGenerator>();

            _mockVisualStudioCodeGenerator.Stub(
                x => x.GenerateCode(Arg<IEnumerable<ICodeGeneratorContext>>.Is.Anything))
                    .Do(
                        (Func<IEnumerable<ICodeGeneratorContext>, IEnumerable<CodeGeneratorResponse>>)
                        (_actualVisualStudioCodeGenerator.GenerateCode)
                    );

            _mockVisualStudioCodeGenerator.Stub(
                x => x.GenerateCode(Arg<IEnumerable<RawSourceFile>>.Is.Anything))
                    .Do(
                        (Func<IEnumerable<RawSourceFile>, IEnumerable<CodeGeneratorResponse>>)
                        (_actualVisualStudioCodeGenerator.GenerateCode)
                    );
        }
    }
}
