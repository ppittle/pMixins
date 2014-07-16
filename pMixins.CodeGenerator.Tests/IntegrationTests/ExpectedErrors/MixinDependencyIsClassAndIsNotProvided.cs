//----------------------------------------------------------------------- 
// <copyright file="MixinDependencyIsClassAndIsNotProvided.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 14, 2014 3:49:01 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.ExpectedErrors
{
    public class MixinDependencyIsClassAndIsNotProvided : ExpectedErrorsTestBase
    {
        protected override bool ExpectNoCodeShouldBeGenerated
        {
            //It is ok if code is generated
            get { return false; }
        }

        protected override string SourceCode
        {
            get
            {
                return
                    @"
                    using CopaceticSoftware.pMixins.Attributes;
                    using CopaceticSoftware.pMixins.Infrastructure;

                    namespace Test
                    {
                        public class Dependency{}

                        public class Mixin : IMixinDependency<Dependency>
                        {
                            public Dependency Dependency { get; set; }
                            
                            //This can be implemented explicitly
                            void IMixinDependency<Dependency>.OnDependencySet() { }
                        }
                        
                        [pMixin(Mixin = typeof(Mixin))]
                        public partial class Target{}
                    }
                ";
            }
        }

        protected override Dictionary<string, Func<CodeGenerationError, bool>> ExpectedErrorMessages
        {
            get
            {
                return new Dictionary<string, Func<CodeGenerationError, bool>>
                       {
                           {
                               Strings.ErrorMixinDependencyIsClassAndIsNotSatisified,
                               error => error.Message == 
                                   string.Format(
                                        Strings.ErrorMixinDependencyIsClassAndIsNotSatisified,
                                        "Test.Mixin",
                                        "Target",
                                        "Test.Dependency")
                           }
                       };
            }
        }
    }
}
