//----------------------------------------------------------------------- 
// <copyright file="MixinIsPrivateNestedTypeInTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 27, 2014 4:10:29 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes
{
    public class MixinIsPrivateNestedTypeInTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class CastingTestShim
                            {       
                                public bool CanImplicitAndExplicitCast()
                                {
                                    BaseClass implicitCast = new Target();
                                    
                                    if (null == implicitCast)
                                        return false;

                                    var explicitCast = (BaseClass)new Target();

                                    return (null != explicitCast);   
                                }
                            }

                            public class BaseClass
                            {
                                public string BaseMethod()
                                {
                                    return ""Base Method"";
                                }
                            }

                            public interface IInterface
                            {
                                string IInterfaceMethod();
                            }                            

                            [CopaceticSoftware.pMixins.Attributes.pMixin(
                                Mixin = typeof (Test.Target.PrivateMixin),
                                ExplicitlyInitializeMixin = true)]
                            public partial class Target
                            {
                                private class PrivateMixin : BaseClass, IInterface
                                {
                                    public PrivateMixin(string name)
                                    {
                                    }

                                    public string MixinMethod()
                                    {
                                        return ""Nested Private Mixin"";
                                    }

                                    public string IInterfaceMethod()
                                    {
                                        return ""IInterfaceMethod"";
                                    }
                                }

                                
                                PrivateMixin 
                                CopaceticSoftware.pMixins.Infrastructure.IMixinConstructorRequirement<PrivateMixin>
                                .InitializeMixin()
                                {
                                    return new PrivateMixin("""");
                                }
                                
                            }                        
                        }";
            }

        }

        [Test]
        public void CanCallAllPublicMethods()
        {
            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "BaseMethod")
                .ShouldEqual("Base Method");

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "IInterfaceMethod")
                .ShouldEqual("IInterfaceMethod");

            CompilerResults
                .ExecuteMethod<string>(
                    "Test.Target",
                    "MixinMethod")
                .ShouldEqual("Nested Private Mixin");
        }

        [Test]
        public void CanCastTargetToBaseClass()
        {
            CompilerResults
                .ExecuteMethod<bool>(
                    "Test.CastingTestShim",
                    "CanImplicitAndExplicitCast")
                .ShouldEqual(true);
        }
    }
}
