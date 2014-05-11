//----------------------------------------------------------------------- 
// <copyright file="MockSolutionTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 2:07:06 PM</date> 
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
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Xml;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Tests.Common;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using Microsoft.Build.Evaluation;
using Ninject;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests
{
    public abstract class MockSolutionTestBase : IntegrationTestBase
    {
        protected TestVisualStudioEventProxy EventProxy;
        /// <summary>
        /// <see cref="IKernel"/> specific to the test class.
        /// It's ok to rebind this Kernel without side effects.
        /// </summary>
        protected IKernel TestSpecificKernel;

        protected IFileWrapper _MockFileWrapper;

        protected IMicrosoftBuildProjectLoader _MockMicrosoftBuildProjectLoader;

        /*
        protected readonly Dictionary<string, string> MockFileWrapperBackingStore = 
            new Dictionary<string, string>(); */

        protected MockSolution _MockSolution;

        /// <summary>
        /// Got tired fo forgetting to use TestSpecificKernel, need a reminder.
        /// </summary>
        protected static new IKernel Kernel
        {
            get { throw new Exception("Don't call Kernel.  Call TestSpecificKernel"); }
        }

        public override void MainSetup()
        {
            TestSpecificKernel = KernelFactory.BuildDefaultKernelForTests();

            EventProxy = TestSpecificKernel.Get<IVisualStudioEventProxy>()
                //This is important, if the casting isn't done, then EventProxy isn't returned via IoC
                as TestVisualStudioEventProxy;

            _MockFileWrapper = BuildMockFileReader();
            TestSpecificKernel.Rebind<IFileWrapper>().ToMethod(c => _MockFileWrapper);

            _MockMicrosoftBuildProjectLoader = BuildMockMicrosoftBuildProjectLoader();
            TestSpecificKernel.Rebind<IMicrosoftBuildProjectLoader>().ToMethod(c => _MockMicrosoftBuildProjectLoader);

            _MockSolution = new MockSolution();

            //Set solution context
            TestSpecificKernel.Get<ISolutionContext>().SolutionFileName =
                _MockSolution.FileName;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            //Simulate a Solution Closing event so Cache classes clear their cache
            EventProxy.FireOnSolutionClosing(this, new EventArgs());
        }

        protected virtual IFileWrapper BuildMockFileReader()
        {
            var fileWrapper = MockRepository.GenerateStub<IFileWrapper>();

            //ReadAllText
            fileWrapper
                .Stub(x => x.ReadAllText(Arg<string>.Is.Anything))
                .Do(
                    (Func<string, string>)
                        (filename =>
                        {
                            var mockFile = _MockSolution.AllMockFiles().FirstOrDefault(x => x.FileName == filename);

                            if (null == mockFile)
                                throw new Exception(
                                    string.Format("Failed to read [{0}].  File does not exist in _MockSolution",
                                        filename));

                            return mockFile.RenderFile();
                        }));
                
            //Exists
            fileWrapper
                .Stub(x => x.Exists(Arg<string>.Is.Anything))
                .Do(
                    (Func<string, bool>)
                        (filename => _MockSolution.AllMockFiles().Any(x => x.FileName == filename)));

            //Delete
            fileWrapper
                .Stub(x => x.Delete(Arg<string>.Is.Anything))
                .Do(
                    (Action<string>)
                        (x => { }));

            //WriteAllText
            fileWrapper
                .Stub(x => x.WriteAllText(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Do(
                    (Action<string, string>)
                        ((filename, text) =>
                         {
                             var existingFile = _MockSolution.AllMockSourceFiles.FirstOrDefault(x => x.FileName == filename);

                             if (null != existingFile)
                                 existingFile.Source = text;
                             else
                             {
                                 //Store file in the Queue, it will be added by IMicrosoftBuildProjectLoader
                                 _newFileWriteQueue.AddOrUpdate(filename, f => text, (x, y) => text);
                             }
                         }));

            return fileWrapper;
        }

        private static readonly object projectLoaderLock = new object();
        private static readonly ConcurrentDictionary<string, string > _newFileWriteQueue = 
            new ConcurrentDictionary<string, string>();
        protected virtual IMicrosoftBuildProjectLoader BuildMockMicrosoftBuildProjectLoader()
        {
            var loader = MockRepository.GenerateStub<IMicrosoftBuildProjectLoader>();

            //LoadMicrosoftBuildProject
            loader.Stub(x => x.LoadMicrosoftBuildProject(Arg<string>.Is.Anything))
                .Do(
                    (Func<string, Project>)
                        (filename =>
                        {
                            lock (projectLoaderLock)
                            {
                                var loadedProjects =
                                    ProjectCollection.GlobalProjectCollection.GetLoadedProjects(filename).ToArray();

                                foreach (var loadedProject in loadedProjects)
                                    ProjectCollection.GlobalProjectCollection.UnloadProject(loadedProject);

                                return
                                    new Project(
                                        new XmlTextReader(new StringReader(_MockFileWrapper.ReadAllText(filename))))
                                    {
                                        FullPath = filename
                                    };
                            }
                        }));
            
            return loader;
        }

        protected bool CanGenerateMixinCodeForSourceFile(MockSourceFile sourceFile, MockProject project = null)
        {
            project = project ?? _MockSolution.Projects[0];

            var result =
                TestSpecificKernel.Get<IVisualStudioCodeGenerator>()
                    .GenerateCode(new[]
                    {
                        new RawSourceFile
                        {
                            FileContents = sourceFile.Source,
                            FileName = sourceFile.FileName,
                            ProjectFileName = project.FileName
                        }
                    })
                    .ToArray();

            if (!result.Any())
                return false;

            var text = result.First().GeneratedCodeSyntaxTree.GetText();

            if (string.IsNullOrEmpty(text))
                return false;

            Console.WriteLine();
            Console.WriteLine("Mixin code behind:");
            Console.WriteLine(text);
            Console.WriteLine();
            Console.WriteLine();

            return true;
        }
    }
}
