//----------------------------------------------------------------------- 
// <copyright file="MixinInterfacesAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    public class MixinInterfacesAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return

                @"
                using System;

                namespace Test
                {
                    public class TestShim
                    {
                        public bool TestConversionFromTargetToMixinInterface()
                        {
                            //Explicit cast
                            var explicitCast = (IMixinParent)new Target();
                            if (null == explicitCast)
                                throw new System.Exception(""Explicit Cast Failed"");
    
                            //Implicit cast
                            IMixinParent implicitCast = new Target();          
                            if (null == implicitCast)
                                throw new System.Exception(""Implicit Cast Failed"");
                                
                            return true;
                        }
                    }

                    public interface IMixinParent
                    {        
                        void ParentMethod();
                    }
                            
                    public interface IMixinChild : IMixinParent
                    {
                        string ChildMethod(DateTime dt);
                    }

                    public interface IMixinOther
                    {
                        void OtherMethod();
                    }

                    public class MixinThatImplementsInterfaces : IMixinChild, IMixinOther
                    {
                        public void ParentMethod(){}
                        public string ChildMethod(DateTime dt) { return ""Hello World""; }
                        public void OtherMethod(){}

                        public void ClassSpecificMethod(){}
                    }
                           
                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinThatImplementsInterfaces))]
                    public partial class Target{}                        
                }";
            }
        }

        [Test]
        public void CanCallInterfaceMethod()
        {
            CompilerResults.ExecuteMethod<string>(
                "Test.Target",
                "ChildMethod",
                ReflectionHelper.DefaultBindingFlags,
                DateTime.Now)
            .ShouldEqual("Hello World");
        }

        [Test]
        public void CanConvertTargetToMixinInterface()
        {
            CompilerResults.ExecuteMethod<bool>(
               "Test.TestShim",
               "TestConversionFromTargetToMixinInterface")
               .ShouldBeTrue();
        }
    }
}
