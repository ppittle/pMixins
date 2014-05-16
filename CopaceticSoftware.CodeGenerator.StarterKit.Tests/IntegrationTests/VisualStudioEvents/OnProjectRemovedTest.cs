//----------------------------------------------------------------------- 
// <copyright file="OnProjectRemovedTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 16, 2014 11:32:10 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    public class OnProjectRemovedTest : MockSolutionTestBase
    {
        private MockProject _projectToRemove;

        public override void MainSetup()
        {
            base.MainSetup();

            _MockSolution.InitializeWithTargetAndMixinInSeparateProjects();
            
            //Warm Caches by loading solution.
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            //Ensure All Classes are  is in the solution
            foreach(var file in _MockSolution.AllMockSourceFiles)
                Assert.True(null != solution.FindCSharpFileByFileName(file.FileName),
                    "{0} was not found in Solution.  Test Environment is not valid.",
                    file.FileName);

            //Simulate Project Item Removed (Basic Class)
            _projectToRemove = _MockSolution.Projects[0];

            _MockSolution.Projects = _MockSolution.Projects.Skip(1).ToList();

            //Fire Project Item Event
            EventProxy.FireOnProjectRemoved(this, new ProjectRemovedEventArgs
            {
                ProjectFullPath = _projectToRemove.FileName
            });
        }

        [Test]
        public void FileShouldBeLoadedIntoSolution()
        {
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(null != solution, "BuildCurrentSolution() returned a null solution.");

            Assert.True(1 == solution.Projects.Count, "Solution did not contain the correct number of Projects (1)");

            foreach (var file in _projectToRemove.MockSourceFiles)
            {
                Assert.IsNull(
                    solution.FindCSharpFileByFileName(file.FileName),
                    "Solution contained File from Removed Project: [{0}]",
                    file.FileName);
            }
        }
    }
}
