﻿//----------------------------------------------------------------------- 
// <copyright file="SimpleCircularReferenceTest.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.pMixins.Tests.Common;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.ExpectedErrors
{
    [FutureRelease]
    public class SimpleCircularReferenceTest : ExpectedErrorsTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                @"
                    using CopaceticSoftware.pMixins.Attributes;

                    namespace Test
                    {
                        [pMixin(Mixin = typeof(Class2))]
                        public partial class Class1{
                            void MethodA(){}                                              
                        }
                        
                        [pMixin(Mixin = typeof(Class1))]
                        public partial class Class2{
                             void MethodB(){}   
                        }
                    }
                ";
            }
        }

        protected override Dictionary<string, Func<CodeGenerationError, bool>> ExpectedErrorMessages
        {
            get
            {
                return new Dictionary<string, Func<CodeGenerationError, bool>>();
            }
        }
    }
}
