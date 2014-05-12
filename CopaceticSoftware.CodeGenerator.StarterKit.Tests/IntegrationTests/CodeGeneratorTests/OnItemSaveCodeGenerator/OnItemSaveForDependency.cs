//----------------------------------------------------------------------- 
// <copyright file="OnItemSaveForDependency.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, May 12, 2014 11:40:02 AM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator
{
    public class OnItemSaveForDependency : OnItemSaveCodeGeneratorTestBase
    {
        private readonly MockSourceFile _mixinSourceFileSecondProject =
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                Source =
                    @"namespace Testing{
                            public class MixinOtherProject{ public int AMethod(){return 1;} }
                        }"
            };

        private readonly MockSourceFile _targetSourceFile =
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                Source =
                    @"namespace Testing{
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(MixinOtherProject))]
                            public partial class Target  {}
                        }"
            };

        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(
                new MockProject()
                {
                    FileName = Path.Combine(MockSolution.MockSolutionFolder, "OtherProject.csproj"),
                    MockSourceFiles =
                    {
                        _mixinSourceFileSecondProject
                    }
                });

            _MockSolution.Projects.Add(
                new MockProject()
                {
                    MockSourceFiles =
                    {
                        _targetSourceFile
                    },
                    ProjectReferences =
                    {
                        _MockSolution.Projects[0]
                    }
                });

            //Simulate Solution Opening event
            EventProxy.FireOnSolutionOpening(this, new EventArgs());

            //Simulate Item Saved
            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _mixinSourceFileSecondProject.FileName,
                    ProjectFullPath = _MockSolution.Projects[1].FileName
                });
        }

        [Test]
        public void CodeBehindFileIsGeneratedAndCompiles()
        {
            Assert.True(
                _MockSolution.AllMockFiles().Any(x => x.FileName.EndsWith("mixin.cs")),
                "Found a mixin.cs code behind file.");

            _MockCodeBehindFileHelper.AssertWasCalled(
                x => x.GetOrAddCodeBehindFile(Arg<string>.Is.Equal(_targetSourceFile.FileName)),
                options => options.Repeat.Twice());

            _MockFileWrapper.AssertWasCalled(
                x => x.WriteAllText(Arg<string>.Is.Equal(_targetSourceFile.FileName), Arg<string>.Is.Anything),
                options => options.Repeat.Twice());

            var compilerResults =
               AssertProjectCompiles(_MockSolution.Projects[1]);

            compilerResults
               .ExecuteMethod<int>(
                   "Testing.Target",
                   "AMethod")
               .ShouldEqual(1); 
        }
    }
}
