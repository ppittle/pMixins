//----------------------------------------------------------------------- 
// <copyright file="GenerateCodeTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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
using System.IO;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Ninject;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.Infrastructure;
using NUnit.Framework;
using Ninject;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class GenerateCodeTestBase : TestBase
    {
        private static readonly string solutionFile =
            Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    @"..\..\..\pMixins.sln"));

        protected readonly string ProjectFile =
            Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    @"..\..\..\pMixins.CodeGenerator.Tests\pMixins.CodeGenerator.Tests.csproj"));

        protected static readonly Solution Solution;

        protected static IKernel Kernel { get; private set; }

        static GenerateCodeTestBase()
        {
            Kernel = new StandardKernel(new StandardModule());

            Solution = Kernel.Get<ISolutionFactory>().BuildSolution(solutionFile);
        }

        protected ICodeGeneratorContextFactory CodeGeneratorContextFactory { get; private set; }

        protected ICodeGeneratorContext CodeGenerationContext { get; private set; }

        protected pMixinPartialCodeGeneratorResponse Response { get; private set; }

        protected string GeneratedCode { get; set; }

        protected Exception Exception { get; set; }
        protected virtual bool ExpectException { get { return false; } }

        protected abstract string SourceCode { get; }

        public override void MainSetup()
        {
            log4net.Config.XmlConfigurator.Configure();

            Console.WriteLine("SourceCode: \n\n"+ SourceCode + "\n");

            var generator = new pMixinPartialCodeGenerator();
            
            try
            {
                CodeGeneratorContextFactory = Kernel.Get<ICodeGeneratorContextFactory>();

                CodeGenerationContext =
                    CodeGeneratorContextFactory
                        .GenerateContext(
                            new []{
                            new RawSourceFile()
                            {
                                FileContents = SourceCode,
                                FileName = "testFile.cs",
                                ProjectFileName = ProjectFile
                            }})
                        .First();

            }
            catch(Exception e)
            {
                Assert.Fail("Exception in CodeGeneratorContextFactory: " + e.Message, e);
            }
            
            try
            {
                Response = generator.GeneratePartialCode(CodeGenerationContext);
            }
            catch (Exception e)
            {
                Exception = e;

                if (!ExpectException)
                    Assert.Fail("Encountered an unexpected exception: " + e.Message, e);
            }

            if (Response.Errors.Any())
            {
                Console.WriteLine("Generator Errors: \n\n");

                Console.WriteLine(
                    string.Join("\r\n",
                        Response.Errors.Select(e => e.Message)));

                Console.WriteLine("\n");
            }

            GeneratedCode = Response.GeneratedCodeSyntaxTree.GetText();

            Console.WriteLine("Generated: \n\n " + GeneratedCode + "\n");
        }
    }
}
