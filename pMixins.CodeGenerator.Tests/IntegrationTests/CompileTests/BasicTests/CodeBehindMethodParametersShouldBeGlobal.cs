//----------------------------------------------------------------------- 
// <copyright file="CodeBehindMethodParametersShouldBeGlobal.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 11, 2014 4:11:22 PM</date> 
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

using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    public class CodeBehindMethodParametersShouldBeGlobal : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get { return @"
                namespace Test
                {
                
                    public class MyEntity{}

                    public abstract class MyEntityMixin
                    {
                        public abstract MyEntity MyEntityMethod(MyEntity entity);
                    }

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (MyEntityMixin))]
                    public partial class Target
                    {                        
                        public MyEntity MyEntityMethodImplementation(MyEntity entity)
                        {
                            return entity;
                        }
                    }  
                } 
            "; }
        }

        [Test]
        public void CanExecuteMethodWithArrays()
        {
            dynamic entity = CompilerResults.TryLoadCompiledType("Test.MyEntity");

            dynamic target = CompilerResults.TryLoadCompiledType("Test.Target");

            object result = target.MyEntityMethod(entity);

            result.ShouldNotBeNull();
        }
    }
}
