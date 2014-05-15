//----------------------------------------------------------------------- 
// <copyright file="OnSolutionOpenWithNoTargetFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 14, 2014 11:13:18 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator
{
    public class OnSolutionOpenWithNoTargetFile : OnItemSaveCodeGeneratorTestBase
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithNormalClassFile();
        }

        [Test]
        public void NoCodeBehindFileIsGenerated()
        {
            this.AssertCodeBehindFileWasNotGenerated();
        }
    }
}
