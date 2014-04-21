//----------------------------------------------------------------------- 
// <copyright file="MixinHasNonParamaterlessConstructorButTargetDoesNotInitialize.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, March 27, 2014 12:42:54 PM</date> 
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

using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    /// <summary>
    /// Abstract Mixin is not Explicitly Initialized.  Should still compile.
    /// </summary>
    [TestFixture]
    public class MixinHasNonParamaterlessConstructorButTargetDoesNotInitialize : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        using CopaceticSoftware.pMixins.Attributes;

                        namespace Test{

                        public abstract class AbstractMixinWithNonParamaterlessConstructor
                        {                           
                            protected AbstractMixinWithNonParamaterlessConstructor(int i)
                            {                         
                            }
                        }

                        [pMixin(Mixin = typeof(AbstractMixinWithNonParamaterlessConstructor))]                        
                        public partial class DoNotInitialize
                        {
                        }
                    }";
            }
        }
    }
}
