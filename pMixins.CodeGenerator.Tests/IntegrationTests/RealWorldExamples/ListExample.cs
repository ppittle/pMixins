//----------------------------------------------------------------------- 
// <copyright file="ListExample.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, August 17, 2014 11:14:42 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.RealWorldExamples
{
    [TestFixture]
    public class ListExample : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get { return @"

                using System;
                using System.Collections.Generic;
                using CopaceticSoftware.pMixins.Attributes;
                

                namespace Test{
                        
                    [pMixin(Mixin = typeof(List<int>))]
                    public partial class Target
                    {
                        public void Add(int item)
                        {
                            __mixins.System_Collections_Generic_List__System_Int32__.Add(42);
                        }
                    }
                }

                "; }
        }

        [Test]
        public void TargetAddMethodIsAlwaysCalled()
        {
            var target = (IList<int>) 
                CompilerResults.TryLoadCompiledType("Test.Target");


            target.Add(1);

            target.Add(2);

            target.Add(4);

            target.ShouldContain(42);
        }
    }
}
