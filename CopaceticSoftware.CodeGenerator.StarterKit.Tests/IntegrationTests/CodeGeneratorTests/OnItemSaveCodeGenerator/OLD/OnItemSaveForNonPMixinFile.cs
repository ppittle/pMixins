//----------------------------------------------------------------------- 
// <copyright file="OnItemSaveForNonPMixinFileWithNoDependencies.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 7:52:01 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OLD
{
    public class OnItemSaveForNonPMixinFile : OnItemSaveCodeGeneratorTestBase
    {
        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(
                new MockProject
                {
                    MockSourceFiles =
                    {
                        MockSourceFile.CreateDefaultFile()
                    }
                });

            //Simulate Solution Opening event
            EventProxy.FireOnSolutionOpening(this, new EventArgs());

            //Simulate Item Saved
            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _MockSolution.Projects[0].MockSourceFiles[0].FileName,
                    ProjectFullPath = _MockSolution.Projects[0].FileName
                });

        }

        [Test]
        public void CodeBehindFileIsNotGenerated()
        {
            Assert.True(
                _MockSolution.AllMockFiles().All(x => !x.FileName.EndsWith("mixin.cs")),
                "Found a mixin.cs code behind file.");

            _MockCodeBehindFileHelper.AssertWasNotCalled(
                x => x.GetOrAddCodeBehindFile(Arg<string>.Is.Anything));

            _MockFileWrapper.AssertWasNotCalled(
                x => x.WriteAllText(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }
    }
}
