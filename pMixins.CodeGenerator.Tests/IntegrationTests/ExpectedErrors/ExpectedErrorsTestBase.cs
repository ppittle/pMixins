//----------------------------------------------------------------------- 
// <copyright file="ExpectedErrorsTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 5:14:08 PM</date> 
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
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.ExpectedErrors
{
    public abstract class ExpectedErrorsTestBase : GenerateCodeTestBase
    {
        protected abstract Dictionary<string, Func<CodeGenerationError, bool>> ExpectedErrorMessages { get; }

        [Test]
        public void GeneratedCodeShouldBeEmpty()
        {
            GeneratedCode.ShouldBeEmpty();
        }

        [Test]
        public void ErrorListShouldContainCorrectErrorMessage()
        {
            Console.WriteLine(
                "Error Messages: {0}{1}{0}{0}",
                Environment.NewLine,
                string.Join(
                    Environment.NewLine,
                    Response.Errors.Select(x => x.Message)
                    ));

            foreach (var expectedMessage in ExpectedErrorMessages)
            {
                Assert.True
                    (Response.Errors.Any(expectedMessage.Value),
                        string.Format("Did not find Error [{0}]", expectedMessage.Key));
            }

        }
    }
}
