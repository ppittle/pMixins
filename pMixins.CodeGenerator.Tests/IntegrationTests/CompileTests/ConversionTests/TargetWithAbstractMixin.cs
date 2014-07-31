﻿//----------------------------------------------------------------------- 
// <copyright file="TargetWithAbstractMixin.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, January 31, 2014 2:02:09 PM</date> 
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

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests
{
    [TestFixture]
    public class TargetWithAbstractMixin : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                namespace Test{
                    public class TestShim{
                        public bool DoConversionTest()
                        {
                            //Explicit cast
                            var explicitCast = (AbstractMixin)new Target();
                            if (null == explicitCast)
                                throw new System.Exception(""Explicit Cast Failed"");
    
                            //Implicit cast
                            AbstractMixin implicitCast = new Target();          
                            if (null == implicitCast)
                                throw new System.Exception(""Implicit Cast Failed"");
                                
                            return true;                               
                        }

                        public string CallAbstractMethodAsAbstractClass()
                        {
                            //Implicit cast
                            AbstractMixin implicitCast = new Target();   

                            return implicitCast.AbstractMethod();      
                        }
                    }

                    public abstract class AbstractMixin
                    {
                        public abstract string AbstractMethod();
                    }

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.AbstractMixin),
                        ExplicitlyInitializeMixin=true)]
                    public partial class Target
                    {
                        public string AbstractMethodImplementation (){return ""Abstract Method!"";}

                        global::Test.AbstractMixin
                        global::CopaceticSoftware.pMixins.Infrastructure.IMixinConstructorRequirement<global::Test.AbstractMixin>.InitializeMixin()
                        {
                            return new pMixins.AutoGenerated.Test.Target.Test.AbstractMixin.AbstractMixinAbstractWrapper(this);
                        }
                    }
                }";
            }
        }

        [Test]
        public void CanConvertTargetToAbstractMixin()
        {
            CompilerResults.ExecuteMethod<bool>(
                "Test.TestShim",
                "DoConversionTest")
                .ShouldBeTrue();

            CompilerResults.ExecuteMethod<string>(
                "Test.TestShim",
                "CallAbstractMethodAsAbstractClass")
                .ShouldEqual("Abstract Method!");
        }
    }
}
