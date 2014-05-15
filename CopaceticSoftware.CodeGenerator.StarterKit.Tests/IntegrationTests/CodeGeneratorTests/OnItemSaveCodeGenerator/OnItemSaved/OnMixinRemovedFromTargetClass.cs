//----------------------------------------------------------------------- 
// <copyright file="OnMixinRemovedFromTargetClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 3:49:24 PM</date> 
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
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnItemSaved
{
    [TestFixture]
    public class OnMixinRemovedFromTargetClass : CodeBehindFileIsGeneratedWithOnItemSaveCodeGenerator
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithTargetAndMixinInSameClass();
        }

        public override void MainSetup()
        {
            base.MainSetup();

            var updatedSource =
                new MockSolution().InitializeWithNormalClassFile()
                    .AllMockSourceFiles.First()
                    .Source;


            this.UpdateMockSourceFileSource(
                _MockSolution.Projects[0].MockSourceFiles[0],
                updatedSource);
        }

        [Test]
        public void EmptyCodeBehindFileIsGenerated()
        {
            this.AssertCodeBehindFileWasGenerated(_MockSolution.Projects[0].MockSourceFiles[0]);

            //Code Behind File should be empty
            _MockSolution.Projects[0].MockSourceFiles[1].Source.ShouldBeEmpty();
        }
    }
}
