//----------------------------------------------------------------------- 
// <copyright file="DisableCodeAttributeOnAssemblyTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 9, 2014 4:42:20 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents;
using Mono.CSharp;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.SolutionScenarios
{
    [TestFixture]
    public class DisableCodeGenerationAttributeOnAssemblyTest : MockSolutionTestBase
    {
        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles =
                {
                    new MockSourceFile
                    {
                        FileName = MockSourceFile.DefaultMockFileName,
                        Source = @"
                                namespace Testing{
                                    
                                    public class Mixin{
                                        public void Method(){}
                                    }

                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]
                                    public partial class Target{}

                                }"
                    }
                }
            });

            Assert.True(
                CanGenerateMixinCodeForSourceFile(
                    _MockSolution.Projects[0].MockSourceFiles[0],
                    _MockSolution.Projects[0]),
                    "Should be able to generate Mixin Code at this point!");

            _MockSolution.Projects[0].MockSourceFiles.Add(
                    new MockSourceFile
                    {
                        FileName = Path.Combine(MockSolution.MockSolutionFolder, "\\Properties\\AssemblyInfo.cs"),
                        Source = @"
                                 [assembly: CopaceticSoftware.pMixins.Attributes.DisableCodeGeneration]
                                    "
                    });

            //Simulate a File Added event
            EventProxy.FireOnProjectItemAdded(this, new ProjectItemAddedEventArgs
            {
                ClassFullPath = _MockSolution.Projects[0].MockSourceFiles[1].FileName,
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });

            //Simulate a Project Reference Removed to force a reload of the project file
            EventProxy.FireOnProjectReferenceRemoved(this, new ProjectReferenceRemovedEventArgs
            {
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });
        }

        [Test]
        public void CodeGenerationIsCancelledWhenDisableCodeAttributeIsPresentInAssemblyInfo()
        {
            Assert.False(
                CanGenerateMixinCodeForSourceFile(
                    _MockSolution.Projects[0].MockSourceFiles[0],
                    _MockSolution.Projects[0]),
                    "Should not be able to generate Mixin Code!");
        }
    }
}
