//----------------------------------------------------------------------- 
// <copyright file="IntegrationTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 12:05:41 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Tests.Common;
using ICSharpCode.NRefactory.Editor;
using Ninject;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests
{
    public abstract class IntegrationTestBase : TestBase
    {
        public readonly static FilePath ProjectFile =
                new FilePath(
                    Directory.GetCurrentDirectory(),
                    @"..\..\..\pMixins.CodeGenerator.Tests\pMixins.CodeGenerator.Tests.csproj");

        public static IKernel Kernel { get; private set; }

        protected Solution Solution { get; set; }

        static IntegrationTestBase()
        {
            Kernel = KernelFactory.BuildDefaultKernelForTests();

            Kernel.Get<ISolutionContext>().SolutionFileName = solutionFile;

            Kernel.Rebind<IFileWrapper>().To<CodeGeneratorFileWrapper>();
        }

        public override void MainSetup()
        {
            base.MainSetup();

            Solution = Kernel.Get<ISolutionFactory>().BuildCurrentSolution();
        }
    }

    public class CodeGeneratorFileWrapper : FileWrapper
    {
        public CodeGeneratorFileWrapper(ICacheEventHelper cacheEventHelper) : base(cacheEventHelper)
        {
        }

        //Return empty text for AssemblyInfo, so the code generator doesn't read the
        //DisableCodeGeneration attribute.
        public override string ReadAllText(FilePath filename)
        {
            if (filename.FullPath.ToLower().EndsWith("assemblyinfo.cs"))
                return string.Empty;

            return base.ReadAllText(filename);
        }
    }
}
