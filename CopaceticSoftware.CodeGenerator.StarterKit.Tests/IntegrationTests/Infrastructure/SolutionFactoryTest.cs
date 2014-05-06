//----------------------------------------------------------------------- 
// <copyright file="SolutionManagerTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 4, 2014 2:53:54 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.Infrastructure
{
    [TestFixture]
    public class SolutionFactoryTest : IntegrationTestBase
    {
        public override void MainSetup()
        {
            try
            {
               Kernel.Get<ISolutionContext>().SolutionFileName = solutionFile;
            }
            catch (Exception e)
            {
                Log.Error(e);

                Assert.Fail("Failed to set Solution Context: " + e.Message);
            }
        }

        [Test]
        public void CanLoadSolutionFile()
        {
            var solution = Kernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(null != solution, "Solution is null after LoadSolution");

            Assert.True(null != solution.Projects, "Solution.Projects is null after LoadSolution");

            Assert.True(solution.Projects.Count > 2, "Solution.Projects is less than expected (2)");
        }
    }
}
