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
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.VisualStudio;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.VSPackage.Tests.IntegrationTests
{
    [pMixin]
    public partial class SampleTarget { }


    [TestFixture]
    public class pMixinsSolutionManagerTest : IntegrationTestBase
    {
        private pMixinsSolutionManager _solutionManager;
        public override void MainSetup()
        {
            try
            {
                _solutionManager = Kernel.Get<pMixinsSolutionManager>();
            }
            catch (Exception e)
            {
                Log.Error(e);

                Assert.Fail("Failed to resolve Solution Manager from IoC: " + e.Message);
            }

            Assert.True(null != _solutionManager, "IoC returned null Solution Manager");

            _solutionManager.LoadSolution(solutionFile);
        }

        [Test]
        public void CanLoadSolutionFile()
        {
            Assert.True(null != _solutionManager.Solution, "Solution is null after LoadSolution");

            Assert.True(null != _solutionManager.Solution.Projects, "Solution.Projects is null after LoadSolution");

            Assert.True(_solutionManager.Solution.Projects.Count > 2, "Solution.Projects is less than expected (2)");
        }

        [Test]
        public void CodeGeneratedFilesIsPopulated()
        {
            Assert.True(
                _solutionManager.CodeGeneratedFiles.Any(c => c.FileName.EndsWith("pMixinsSolutionManagerTest.cs")),
                "Code Generated Files did not find SampleTarget file.");
        }
    }
}
