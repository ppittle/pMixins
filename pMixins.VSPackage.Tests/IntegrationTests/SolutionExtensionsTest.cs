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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using NBehave.Spec.NUnit;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.VSPackage.Tests.IntegrationTests
{
    public class DummyMixin{}

    [pMixin(Mixin = typeof(DummyMixin))]
    public partial class SampleTarget { }


    [TestFixture]
    public class SolutionExtensionsTest : IntegrationTestBase
    {
        private Solution _solution;

        private CSharpFile _thisFile;

        public override void MainSetup()
        {
            Kernel.Get<ISolutionContext>().SolutionFileName = solutionFile;

            _solution = Kernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(null != _solution, "Failed loading Solution from [{0}]", solutionFile);

            _thisFile = 
                _solution
                    .GetValidPMixinFiles()
                    .FirstOrDefault(c => c.FileName.EndsWith("SolutionExtensionsTest.cs"));
        }

        [Test]
        public void GetValidPMixinFilesFindsThisFile()
        {
            Assert.True(null != _thisFile,
                "Code Generated Files did not find SampleTarget file.");
        }

        [Test]
        public void CanCorrectlyResolveSampleTargetPMixinAttributes()
        {
            Assert.True(null != _thisFile,
                "Can't run test when _thisFile is null.");

            var sampleTargetClassDefinition = _thisFile.SyntaxTree.GetPartialClasses().First();

            var sampleTargetType = _thisFile.CreateResolver().Resolve(sampleTargetClassDefinition);

            var sampleTargetTypeAttributes =
                sampleTargetType.Type.GetAttributes()
                .Where(x => x.AttributeType.Implements<IpMixinAttribute>());

            sampleTargetTypeAttributes.Count().ShouldEqual(1);
        }
    }
}
