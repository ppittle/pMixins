//----------------------------------------------------------------------- 
// <copyright file="OnBuildCodeGeneratorTests.cs" company="Copacetic Software"> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using EnvDTE;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnBuildCodeGenerator
{
    public class OnBuildCodeGeneratorTests : OnSolutionOpenCodeGeneratorTestBase
    {
        // ReSharper disable NotAccessedField.Global
        //Instantiate the OnSolutionOpenCodeGenerator so it will subscribe to events
        protected pMixinsOnBuildCodeGenerator _PMixinsOnBuildCodeGenerator;
        // ReSharper restore NotAccessedField.Global

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSeparateClass();
        }

        public override void MainSetup()
        {
            base.MainSetup();

            //Verify OnSolutionOpen generated code behind
            this.AssertCodeBehindFileWasGenerated(
                _MockSolution.Projects[0].MockSourceFiles[1]);

            _PMixinsOnBuildCodeGenerator = TestSpecificKernel.Get<pMixinsOnBuildCodeGenerator>();

            //Delete the code behind
            _MockSolution.RemoveFile(s =>
                s.AllMockSourceFiles.First(f => f.FileName.EndsWith(".mixin.cs")));

            //Fire OnBuild
            EventProxy.FireOnBuildBegin(this, 
                new VisualStudioBuildEventArgs
                {
                    BuildAction = vsBuildAction.vsBuildActionBuild,
                    ProjectFullPath = _MockSolution.Projects[0].FileName,
                    Scope = vsBuildScope.vsBuildScopeSolution
                });
        }

        [Test]
        public void CanExecuteMixedMethod()
        {
            //Verify can Execute a Mixed in Method
            _MockSolution.Projects[0].MockSourceFiles[1]
                .AssertCompilesAndCanExecuteMethod(_MockSolution);
        }
    }
}
