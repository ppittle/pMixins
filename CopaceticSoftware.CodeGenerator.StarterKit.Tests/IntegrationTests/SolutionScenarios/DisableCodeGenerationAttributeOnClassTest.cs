//----------------------------------------------------------------------- 
// <copyright file="DisableCodeGenerationAttributeOnClassTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 9, 2014 4:51:44 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.SolutionScenarios
{
    [TestFixture]
    public class DisableCodeGenerationAttributeOnClassTest : MockSolutionTestBase
    {
        private const string _sourceFormat =
            @"
                namespace Testing{{
                                    
                    public class Mixin{{
                        public void Method(){{}}
                    }}
                    
                    {0}
                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]
                    public partial class Target{{}}

                }}";

        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles =
                {
                    new MockSourceFile
                    {
                        Source = string.Format(
                            _sourceFormat,
                            //Don't add DisableCodeGenerator attribute yet
                            "")
                    }
                }
            });

            Assert.True(
                CanGenerateMixinCodeForSourceFile(
                    _MockSolution.Projects[0].MockSourceFiles[0],
                    _MockSolution.Projects[0]),
                "Should be able to generate Mixin Code at this point!");

            //Add DisableCodeGenerator
            _MockSolution.Projects[0].MockSourceFiles[0].Source =
                string.Format(
                    _sourceFormat,
                    "[CopaceticSoftware.pMixins.Attributes.DisableCodeGeneration");

            //Simulate a File Saved event
            EventProxy.FireOnProjectItemSaved(this, new ProjectItemSavedEventArgs
            {
                ClassFullPath = _MockSolution.Projects[0].MockSourceFiles[0].FileName,
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });
        }

        [Test]
        public void CodeGenerationIsCancelledWhenTargetIsDecoratedWithDisableCodeGeneratorAttribute()
        {
            Assert.False(
                CanGenerateMixinCodeForSourceFile(
                    _MockSolution.Projects[0].MockSourceFiles[0],
                    _MockSolution.Projects[0]),
                "Should not be able to generate Mixin Code!");
        }
    }
}
