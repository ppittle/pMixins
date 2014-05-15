//----------------------------------------------------------------------- 
// <copyright file="OnSolutionOpeningTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 7:41:39 PM</date> 
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
using System.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OLD
{
    public class OnSolutionOpeningTest : OnItemSaveCodeGeneratorTestBase
    {
        private readonly MockSourceFile _mixinSourceFile = 
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin.cs"),
                Source = 
                    @"namespace Testing{
                            public class Mixin{ public void AMethod(){} }
                        }"
            };

        private readonly MockSourceFile _mixinSourceFileSecondProject =
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                Source =
                    @"namespace Testing{
                            public class MixinOtherProject{ public void AMethod(){} }
                        }"
            };

        private readonly MockSourceFile _targetSourceFile = 
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                Source =
                    @"namespace Testing{
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(MixinOtherProject))]
                            public partial class Target  {}
                        }"
            };

        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(new MockProject()
                                       {
                                           FileName = Path.Combine(MockSolution.MockSolutionFolder, "OtherProject.csproj"),
                                           MockSourceFiles =
                                           {
                                               _mixinSourceFileSecondProject
                                           }
                                       });

            _MockSolution.Projects.Add(new MockProject()
                                       {
                                           MockSourceFiles =
                                           {
                                               _mixinSourceFile,
                                               _targetSourceFile
                                           },
                                           ProjectReferences =
                                           {
                                               _MockSolution.Projects[0]
                                           }
                                       });

            //Simulate Solution Opening event
            EventProxy.FireOnSolutionOpening(this, new EventArgs());
        }

        [Test]
        public void TargetIsUpdatedWhenMixinInSameProjectIsSaved()
        {
            //Set Expectations
            var generatedFile = _targetSourceFile.FileName.Replace(".cs", ".mixin.cs");

            _MockCodeBehindFileHelper.Expect(
                x => x.GetOrAddCodeBehindFile(
                    Arg<string>.Is.Equal(_targetSourceFile)));

            _MockFileWrapper.Expect(
                x => x.WriteAllText(
                    Arg<string>.Is.Equal(generatedFile),
                    Arg<string>.Is.NotNull));

            //Simulate Save on _mixinSourceFile
            EventProxy.FireOnProjectItemSaved(this, 
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _mixinSourceFile.FileName,
                    ProjectFullPath = _MockSolution.Projects[1].FileName
                });

            //Verify Expectations
            _MockFileWrapper.VerifyAllExpectations();
            _MockCodeBehindFileHelper.VerifyAllExpectations();
        }

        [Test]
        public void TargetIsUpdatedWhenMixinInReferencedProjectIsSaved()
        {
            //Set Expectations
            var generatedFile = _targetSourceFile.FileName.Replace(".cs", ".mixin.cs");
            
            _MockCodeBehindFileHelper.Expect(
                x => x.GetOrAddCodeBehindFile(
                    Arg<string>.Is.Equal(_targetSourceFile)));

            _MockFileWrapper.Expect(
                x => x.WriteAllText(
                    Arg<string>.Is.Equal(generatedFile),
                    Arg<string>.Is.NotNull));

            //Simulate Save on _mixinSourceFileSecondProject
            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _mixinSourceFileSecondProject.FileName,
                    ProjectFullPath = _MockSolution.Projects[0].FileName
                });

            //Verify Expectations
            _MockFileWrapper.VerifyAllExpectations();
            _MockCodeBehindFileHelper.VerifyAllExpectations();
        }
    }
}
