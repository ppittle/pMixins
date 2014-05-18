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

using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.SolutionScenarios
{
    [TestFixture]
    public class DisableCodeGenerationAttributeOnAssemblyTest : OnSolutionOpenCodeGeneratorTestBase
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSameClass()
               .Projects[0].MockSourceFiles.Add(
                   new MockSourceFile
                   {
                       Source = @"
                                namespace Testing{
                                    
                                    public class Mixin{
                                        public void Method(){}
                                    }

                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]
                                    public partial class Target{}

                                }"
                   });
        }

        [Test]
        public void CodeGenerationIsCancelledWhenDisableCodeAttributeIsPresentInAssemblyInfo()
        {
            this.AssertCodeBehindFileWasNotGenerated();

            Assert.False(
                CanGenerateMixinCodeForSourceFile(
                    _MockSolution.Projects[0].MockSourceFiles[0],
                    _MockSolution.Projects[0]),
                    "Should not be able to generate Mixin Code!");
        }
    }
}
