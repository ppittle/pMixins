//----------------------------------------------------------------------- 
// <copyright file="OnProjectItemRemovedTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 3:11:54 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    public class OnProjectItemRemovedTest : VisualStudioEventTestBase
    {
        private static readonly MockSourceFile _sourceFile = MockSourceFile.CreateDefaultFile();

        public override void MainSetup()
        {
            base.MainSetup();

            // Set Initial Solution State
            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles = { _sourceFile }
            });
            
            //Warm Caches by loading solution.
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            //Ensure Basic Class is in the solution
            Assert.True(null != GetBasicFile(solution), "Basic File was already in Solution.  Test Environment is not valid.");

            //Simulate Project Item Removed (Basic Class)
            _MockSolution.Projects[0].MockSourceFiles.Clear();
            
            //Fire Project Item Event
            EventProxy.FireOnProjectItemRemoved(this, new ProjectItemRemovedEventArgs
            {
                ClassFullPath = _sourceFile.FileName,
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });
        }

        [Test]
        public void FileShouldBeLoadedIntoSolution()
        {
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(null != solution, "BuildCurrentSolution() returned a null solution.");

            var csharpBasicFile = GetBasicFile(solution);

            Assert.True(null == csharpBasicFile, "Solution contained Basic Class File");
        }

        private CSharpFile GetBasicFile(Solution s)
        {
            return s.AllFiles
                    .FirstOrDefault(f => f.FileName.Equals(_sourceFile.FileName));

        }
    }
}
