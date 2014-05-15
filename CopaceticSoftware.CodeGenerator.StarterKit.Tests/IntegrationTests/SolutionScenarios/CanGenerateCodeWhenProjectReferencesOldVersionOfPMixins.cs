//----------------------------------------------------------------------- 
// <copyright file="CanGenerateCodeWhenProjectReferencesOldVersionOfPMixins.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 4:54:51 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.SolutionScenarios
{
    [TestFixture]
    public class CanGenerateCodeWhenProjectReferencesOldVersionOfPMixins : OnSolutionOpenCodeGeneratorTestBase
    {
        private string oldPMixinsDllPath;

        protected override void MainSetupInitializeSolution()
        {
            oldPMixinsDllPath =
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        @"..\..\..\CopaceticSoftware.CodeGenerator.StarterKit.Tests\Data\CopaceticSoftware.pMixins.dll"));

            _MockSolution.InitializeWithTargetAndMixinInSameClass();

            //Update References
            _MockSolution.Projects[0].AssemblyReferences =
                _MockSolution.Projects[0].AssemblyReferences
                    .Where(r => !r.ToLower().Contains("pmixin"))
                    .Union(new[] {oldPMixinsDllPath})
                    .ToArray();
        }

        [Test]
        public virtual void CodeBehindFilesAreGenerated()
        {
            this.AssertCodeBehindFileWasGenerated(_MockSolution.Projects[0].MockSourceFiles[0]);
        }

        [Test]
        public void CodeBehindCompilesWithoutErrors()
        {
            _MockSolution.Projects[0].AssertCompiles();
        }
    }
}
