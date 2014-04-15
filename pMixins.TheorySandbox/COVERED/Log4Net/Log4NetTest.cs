//----------------------------------------------------------------------- 
// <copyright file="Log4NetTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, November 9, 2013 7:39:31 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Log4Net
{
    /// <summary>
    /// Covered in:
    ///     <see cref="CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests.Log4NetMixin"/>
    /// </summary>
    public class Log4NetTest : SpecTestBase
    {
        private Log4NetSpec _spec;

        protected override void Establish_context()
        {
            _spec = new Log4NetSpec();
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests.Log4NetMixin.CanCallMethodThatLogs"/>
        /// </summary>
        [Test]
        public void Can_Call_Method_That_Logs()
        {
            _spec.MethodThatLogs();
        }
    }
}
