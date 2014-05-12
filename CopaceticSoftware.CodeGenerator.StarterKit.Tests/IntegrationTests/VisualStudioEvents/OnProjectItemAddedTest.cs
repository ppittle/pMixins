//----------------------------------------------------------------------- 
// <copyright file="OnProjectItemAddedToExistingProject.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 12:57:02 PM</date> 
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
using System.Threading;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using Ninject;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    public class OnProjectItemAddedTest : MockSolutionTestBase
    {
        private static readonly MockSourceFile _sourceFileAdded = MockSourceFile.CreateDefaultFile();
        private readonly string _sourceFileClass = Path.GetFileNameWithoutExtension(_sourceFileAdded.FileName);

        public override void MainSetup()
        {
            base.MainSetup();

            // Set Initial Solution State
            _MockSolution.Projects.Add(new MockProject());

            //Warm Caches by loading solution.
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            //Ensure Basic Class isn't yet in the solution
            Assert.True(null == solution.FindCSharpFileByFileName(_sourceFileAdded.FileName), 
                "Basic File was already in Solution.  Test Environment is not valid.");

            //Simulate Project Item added (Basic Class)
            _MockSolution.Projects[0].MockSourceFiles.Add(_sourceFileAdded);

            //Fire Project Item Event
            EventProxy.FireOnProjectItemAdded(this, new ProjectItemAddedEventArgs
            {
                ClassFullPath = _sourceFileAdded.FileName,
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });
            
            //Wait a Second for the Async reader to catch up.
            Thread.Sleep(1000);

            _MockFileWrapper.AssertWasCalled(f => f.ReadAllText(Arg.Is(_sourceFileAdded.FileName)));
        }

        [Test]
        public void FileShouldBeLoadedIntoSolution()
        {
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(null != solution, "BuildCurrentSolution() returned a null solution.");

            var csharpBasicFile = solution.FindCSharpFileByFileName(_sourceFileAdded.FileName);
               
            Assert.True(null != csharpBasicFile, "Solution did not contain Basic Class File");

            Assert.True(
                csharpBasicFile.ResolveTypes()
                    .Any(x => x.FullName.EndsWith(_sourceFileClass)),
                "Failed to Resolve " + _sourceFileClass + " IType");
        }
    }
}
