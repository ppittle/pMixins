//----------------------------------------------------------------------- 
// <copyright file="ExistingMixinFileTest.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Tests.Common;
using NBehave.Spec.NUnit;
using Ninject;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ExistingFile
{
    public class ExistingMixin
    {
    }

    [pMixin(Mixin = typeof(ExistingMixin))]
    public partial class Target { }

    /// <summary>
    /// Tests running the compilation on a Source file (this source file)
    /// that is part of a project
    /// </summary>
    public class ExistingMixinFileTest : IntegrationTestBase
    {
        private pMixinPartialCodeGeneratorResponse _response;

        public override void MainSetup()
        {
            base.MainSetup();

            var generator = new pMixinPartialCodeGenerator();

            var context =
                Kernel.Get<ICodeGeneratorContextFactory>()
                    .GenerateContext(
                        Solution.AllFiles
                            .Where(f => f.FileName.EndsWith("ExistingMixinFileTest.cs")))
                    .First();

            _response = generator.GeneratePartialCode(context);
        }

        [Test]
        public void CodeShouldGenerateWithoutErrors()
        {
            if (_response.Errors.Any())
                Assert.Fail(
                    String.Join(Environment.NewLine,
                    _response.Errors.Select(x => x.Message)));
        }

        [Test]
        public void CodeShouldBeGenerated()
        {
            Console.WriteLine(_response.GeneratedCodeSyntaxTree.GetText());

            _response.GeneratedCodeSyntaxTree.GetText().ShouldNotBeEmpty();
        }
    }
}
