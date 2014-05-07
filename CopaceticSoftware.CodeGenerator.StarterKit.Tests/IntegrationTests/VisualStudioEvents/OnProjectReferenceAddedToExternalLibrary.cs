//----------------------------------------------------------------------- 
// <copyright file="OnProjectReferenceAddedToExternalLibrary.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 6:35:12 PM</date> 
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
    public class SimpleObject { }

    public class OnProjectReferenceAddedToExternalLibrary : VisualStudioEventTestBase
    {
        private readonly MockSourceFile _sourceFile = MockSourceFile.CreateDefaultFile();
        private const string _sourceFileClass = "SimpleObjectChild";


        public override void MainSetup()
        {
            base.MainSetup();

            _sourceFile.Source =
                string.Format(
                    @"
                        namespace Testing
                        {{
                            public class {0}  {{ 
                                public void Method({1}){{}} 
                            }}
                        }}",
                    _sourceFileClass,
                    typeof (SimpleObject).FullName);

            // Set Initial Solution State
            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles = new []{_sourceFile}
            });

            //Warm Caches by loading solution.
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            var blah = solution.FindCSharpFileByFileName(_sourceFile.FileName)
                .ResolveTypes();

            //Make sure _sourceFileClass does not resolve
            Assert.True(
                !solution.FindCSharpFileByFileName(_sourceFile.FileName)
                    .ResolveTypes().Any(),
                "Should not be able to resolve _sourceFileClass yet!  Was the file built correctly?");

            //Simulate Project Reference Added
            var referencePath = 
                ReferenceHelper.GetReferenceToDllInTestProject(
                    typeof (SimpleObject).Assembly.FullName);

            _MockSolution.Projects[0].AssemblyReferences.Add(referencePath);

            //Fire Project Reference Added Event
            EventProxy.FireOnProjectReferenceAdded(this, new ProjectReferenceAddedEventArgs
            {
                ProjectFullPath = _MockSolution.Projects[0].FileName,
                ReferencePath = referencePath
            });
        }

        [Test]
        public void ClassWithExternalReferenceResolvesAfterReferencesHasBeenAdded()
        {
            var solution = TestSpecificKernel.Get<ISolutionFactory>().BuildCurrentSolution();

            Assert.True(
                solution.FindCSharpFileByFileName(_sourceFile.FileName)
                    .ResolveTypes().Any(),
                "Failed to resolve _sourceFileClass");
        }
    }
}
