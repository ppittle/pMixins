//----------------------------------------------------------------------- 
// <copyright file="NewDependenciesAreDiscoveredTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, May 12, 2014 5:31:23 PM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator
{
    public class NewDependenciesAreDiscoveredTest : OnItemSaveCodeGeneratorTestBase
    {
        private const string _updatedMixinFileSource =
            @"namespace Testing{
                            public class MixinOtherProject{ 
                                public void AMethod(){} 
                                public int NumberMethod(){return 42;}
                            }
                }";
        
        private readonly MockSourceFile _mixinSourceFile =
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                Source =
                    @"namespace Testing{
                            public class MixinOtherProject{ public void AMethod(){} }
                        }"
            };

        private const string _updatedTargetSourceFileSource =
            @"namespace Testing{
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(MixinOtherProject))]
                            public partial class Target  {}
                        }";

        private readonly MockSourceFile _targetSourceFile =
            new MockSourceFile
            {
                FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                Source =""
            };


        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(
                new MockProject
                {
                    FileName = Path.Combine(MockSolution.MockSolutionFolder, "OtherProject.csproj"),
                    MockSourceFiles =
                    {
                        _mixinSourceFile
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
                    },
                    AssemblyReferences =
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(
                            new []
                            {
                                ReferenceHelper.GetReferenceToPMixinsDll()
                            })
                        .ToList()
                });

            //Simulate Solution Opening event
            EventProxy.FireOnSolutionOpening(this, new EventArgs());

            //Simulate Item Saved
            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _mixinSourceFile.FileName,
                    ProjectFullPath = _MockSolution.Projects[1].FileName
                });

            //Update _targetSourceFile to have a Mixin
            _targetSourceFile.Source = _updatedTargetSourceFileSource;

            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _targetSourceFile.FileName,
                    ProjectFullPath = _MockSolution.Projects[1].FileName
                });

            //Update _mixinSourceFile to have a new Method
            _mixinSourceFile.Source = _updatedMixinFileSource;

            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _mixinSourceFile.FileName,
                    ProjectFullPath = _MockSolution.Projects[0].FileName
                });
        }

        [Test]
        public void CodeBehindFileIsGeneratedAndCompiles()
        {
            Assert.True(
                _MockSolution.AllMockFiles().Any(x => x.FileName.EndsWith("mixin.cs")),
                "Found a mixin.cs code behind file.");

            var compilerResults =
                AssertProjectCompiles(_MockSolution.Projects[1]);

            compilerResults
                .ExecuteMethod<int>(
                    "Testing.Target",
                    "NumberMethod")
                .ShouldEqual(42); 
        }
    }
}
