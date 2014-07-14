//----------------------------------------------------------------------- 
// <copyright file="GenerateCodeAndCompileTestBase.cs" company="Copacetic Software"> 
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using Microsoft.CSharp;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests
{
    [TestFixture]
    public abstract class GenerateCodeAndCompileTestBase : GenerateCodeTestBase
    {
        protected virtual bool FailOnCompilerErrors { get { return true; } }

        protected CompilerResults CompilerResults { get; private set; }

        private bool _hasCompilerErrors = false;
        
        public override void MainSetup()
        {
            base.MainSetup();

            var project = Solution.Projects.FirstOrDefault(x => x.FileName.Equals(ProjectFile));

            Assert.True(null != project, "Failed to load project from Solution.Projects.  This is a bug with the Test");

            Assert.False(string.IsNullOrEmpty(GeneratedCode), "No Code was Generated");

            //http://stackoverflow.com/questions/826398/is-it-possible-to-dynamically-compile-and-execute-c-sharp-code-fragments
            var csc = new CSharpCodeProvider(
                new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

            var referencedAssemblies =
                    AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.FullName.StartsWith("mscorlib", StringComparison.InvariantCultureIgnoreCase))
                    .Where(a => !a.IsDynamic) //necessary because a dynamic assembly will throw and exception when calling a.Location.  But unsure why there are dynamic assemblies in the Current Domain
                    .Select(a => a.Location)
                    .Union(AddAdditionalReferencedAssemblies())
                    .Distinct()
                    .ToArray();

            var parameters = new CompilerParameters(
                referencedAssemblies);

            CompilerResults = csc.CompileAssemblyFromSource(parameters,
                SourceCode + GeneratedCode);

            var errors = CompilerResults.PrettyPrintErrorList();

            if (!string.IsNullOrEmpty(errors))
            {
                _hasCompilerErrors = true;

                var logMessage = "Compilation Errors: \r\n: " + errors;

                Console.WriteLine(logMessage);

                if (FailOnCompilerErrors)
                    Assert.Fail();
            }
        }

        protected virtual IEnumerable<string> AddAdditionalReferencedAssemblies()
        {
            return Enumerable.Empty<string>();
        }

        [Test]
        public void ShouldNotHaveUnexpectedCompilerErrors()
        {
            if (FailOnCompilerErrors)
                Assert.True(!_hasCompilerErrors);
            else
            {
                Assert.True(_hasCompilerErrors);
            }
        }
    }
}
