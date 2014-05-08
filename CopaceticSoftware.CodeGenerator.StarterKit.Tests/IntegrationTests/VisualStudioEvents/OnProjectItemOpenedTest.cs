//----------------------------------------------------------------------- 
// <copyright file="OnProjectItemOpenedTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 8, 2014 8:41:17 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    [TestFixture]
    public class OnProjectItemOpenedTest : VisualStudioEventTestBase
    {
        private readonly MockSourceFile _sourceFile = MockSourceFile.CreateDefaultFile();

        private const string SourceCodeFormat =
            @"
            namespace Testing
            {{
                public class Mixin{{ public void Method(){{ }} }} 

                [ CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]
                public {0} class Target  {{ 
                          
                }}
            }}";

        private IVisualStudioOpenDocumentReader _documentReader;

        public override void MainSetup()
        {
            base.MainSetup();

            _sourceFile.Source =
                string.Format(
                    SourceCodeFormat,
                    //don't add the keyword partial, so code generator fails
                    "");

            // Set Initial Solution State
            _MockSolution.Projects.Add(new MockProject
            {
                MockSourceFiles = new[] {_sourceFile}
            });

            _documentReader = buildMockOpenDocumentReader();

            //Verify can't generate code
            Assert.False(CanGenerateMixinCodeForSourceFile(_sourceFile),
                "Should not be able to Generate Code at this point.  Does the _sourceFile contain an error like it should?");

            //Verify _documentReader has not been called.
            _documentReader.AssertWasNotCalled(x => x.GetDocumentText());

            //Make sure MockFileWrapper is not called to read _sourceFile
            MockFileWrapper.Expect(x => x.ReadAllText(Arg.Is(_sourceFile.FileName)))
                .Repeat.Never();

            //Simulate firing Document Open Event
            EventProxy.FireOnProjectItemOpened(this, new ProjectItemOpenedEventArgs
            {
                ClassFullPath = _sourceFile.FileName,
                DocumentReader = _documentReader,
                ProjectFullPath = _MockSolution.Projects[0].FileName
            });
        }

        [Test]
        public void CanGenerateCodeFromOpenDocumentWindow()
        {
            Assert.True(
                CanGenerateMixinCodeForSourceFile(_sourceFile),
                "Failed to build Mixin code _sourceFile");

            MockFileWrapper.VerifyAllExpectations();

            //Make sure the file was loaded from document
            _documentReader.AssertWasCalled(x => x.GetDocumentText());
        }

        private IVisualStudioOpenDocumentReader buildMockOpenDocumentReader()
        {
            var reader = MockRepository.GenerateMock<IVisualStudioOpenDocumentReader>();

            reader.Stub(x => x.GetDocumentText())
                //return the compilable source code
                .Return(
                    string.Format(
                        SourceCodeFormat,
                        //add the keyword partial, so code generator succeeds
                        "partial"));

            return reader;
        }
    }
}
