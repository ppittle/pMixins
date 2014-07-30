//----------------------------------------------------------------------- 
// <copyright file="OnInvalidTargetAddedToEmptyProject.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 2:09:06 PM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnProjectItemAdded
{
    public class OnInvalidTargetAddedToEmptyProject : OnItemSaveCodeGeneratorTestBase
    {
        private MockSourceFile _fileToAdd;

        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithInvalidTargetFile();

            _fileToAdd =
                _MockSolution.RemoveFile(s => s.AllMockSourceFiles.First());
        }

        public override void MainSetup()
        {
            base.MainSetup();

            this.AssertCodeBehindFileWasNotGenerated();

            //Add Invalid Class
            this.AddMockSourceFile(s => s.Projects[0], _fileToAdd);
        }

        //Logic updated.  Code Behind file is not created 
        //on error in target.
        [Test]
        public void NoCodeBehindFileIsGenerated()
        {
            this.AssertCodeBehindFileWasNotGenerated();
        }
    }
}
