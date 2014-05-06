//----------------------------------------------------------------------- 
// <copyright file="WhenThereIsAParseErrorLaterInTheFile.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 5:23:01 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ParsingTests
{

    public class WhenThereIsAParseErrorLaterInTheFileTest : GenerateCodeTestBase
    {
        public override void MainSetup()
        {
            base.MainSetup();
           
        }

        protected override string SourceCode
        {
            get
            {
                return
                    @"
                    namespace Test
                    {
                        public class MixinWithPublicMethod
                        {
                            public string PublicMethod()
                            {
                                return ""Public Method"";
                            }             
                        }

                        [CopaceticSoftware.pMixins.Attributes.pMixin(
                            Mixin = typeof (Test.MixinWithPublicMethod))]
                        public partial class Target{}       

                        public class ClassWithSyntaxError
                        {
                            int missingSemicolon
                        }               
                    }
                    ";
            }
        }

        [Test]
        public void CanCallPublicMethodOnBothTargets()
        {
            base.GeneratedCode.ShouldContain("Target");
        }
    }

}
