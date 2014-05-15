//----------------------------------------------------------------------- 
// <copyright file="OnSolutionItemOpenCodeGeneratorTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 12:02:06 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using Ninject;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator
{
    public abstract class OnSolutionItemOpenCodeGeneratorTestBase : MockSolutionTestBase
    {
        // ReSharper disable NotAccessedField.Global
        //Instantiate the OnSolutionOpenCodeGenerator so it will subscribe to events
        protected pMixinsOnSolutionOpenCodeGenerator _PMixinsOnSolutionOpenCodeGenerator;
        // ReSharper restore NotAccessedField.Global

        public IVisualStudioCodeGenerator _mockVisualStudioCodeGenerator;

        public VisualStudioCodeGenerator _actualVisualStudioCodeGenerator;

        public override void MainSetup()
        {
            base.MainSetup();

            _PMixinsOnSolutionOpenCodeGenerator = TestSpecificKernel.Get<pMixinsOnSolutionOpenCodeGenerator>();

            SetupCodeGeneratorMock();

            TestSpecificKernel.Rebind<IVisualStudioCodeGenerator>().ToMethod(x => _mockVisualStudioCodeGenerator);

            MainSetupInitializeSolution();

            this.FireSolutionOpen();
        }

        protected abstract void MainSetupInitializeSolution();

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
