//----------------------------------------------------------------------- 
// <copyright file="OnProjectReferenceAddedToProject.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 8, 2014 9:17:16 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    [TestFixture]
    public class OnProjectReferenceAddedToProjectTest : MockSolutionTestBase
    {
        private readonly MockSourceFile _sourceFile = MockSourceFile.CreateDefaultFile();

        public override void MainSetup()
        {
            base.MainSetup();

            #region Source Files
            var newProjectSourceFile = new MockSourceFile
            {
                FileName = new FilePath(MockSolution.MockSolutionFolder, "NewProjectSourceFile.cs"),
                Source =
                    @"namespace Testing{
                        public class NewProjectClass{ public void AMethod(){} }
                      }"
            };

            _sourceFile.Source =
               @" namespace Testing
            {
                [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(NewProjectClass))]
                public partial class Target  {
                          
                }
            }";
            #endregion

            // Set Initial Solution State
            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles = new[] { _sourceFile }
            });

            this.FireSolutionOpen();

            //Code Generator should not be able to generate Mixin code yet
            Assert.False(
                CanGenerateMixinCodeForSourceFile(_sourceFile),
                "Should not be able to generate Mixin code for _sourceFile yet!  Was the file built correctly?");

            //Simulate adding a new Project
            _MockSolution.Projects.Add(
                new MockProject
                {
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "NewProject.csproj"),
                    MockSourceFiles = new[] { newProjectSourceFile }
                });

            EventProxy.FireOnProjectAdded(this, new ProjectAddedEventArgs
            {
                ProjectFullPath = _MockSolution.Projects[1].FileName
            });
          
            //Simulate adding a reference to the new Project
            _MockSolution.Projects[0].ProjectReferences.Add(
                _MockSolution.Projects[1]);

            EventProxy.FireOnProjectReferenceAdded(this, new ProjectReferenceAddedEventArgs
            {
                ProjectFullPath = _MockSolution.Projects[0].FileName,
                ReferencePath = _MockSolution.Projects[1].FileName
            });

            //Expect Project File will be reread (by test AssemblyResolver)
            _MockFileWrapper.Expect(x => x.ReadAllText(Arg.Is(_MockSolution.Projects[0].FileName)));
        }

        [Test]
        public void ClassWithExternalReferenceResolvesAfterReferencesHasBeenAdded()
        {
            Assert.True(
                CanGenerateMixinCodeForSourceFile(_sourceFile),
                "Failed to build Mixin code _sourceFile");

            _MockFileWrapper.VerifyAllExpectations();
        }
    }
}
