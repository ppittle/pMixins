//----------------------------------------------------------------------- 
// <copyright file="ExpectedOutputShouldMatchGeneratedCode.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Helpers;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.Infrastructure
{
    public abstract class ExpectedOutputShouldMatchGeneratedCode : GenerateCodeTestBase
    {
        protected abstract string ExpectedOutput { get; }

        [Test]
        public virtual void ExpectedOutputShouldMatch()
        {
            Console.WriteLine("Expected: \n\n " + ExpectedOutput + "\n");

            Console.WriteLine("Generated: \n\n " + GeneratedCode + "\n");

            SourceCodeHelper.CleanCode(GeneratedCode).ShouldEqual(
                SourceCodeHelper.CleanCode(ExpectedOutput));
        }
    }
}
