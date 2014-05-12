//----------------------------------------------------------------------- 
// <copyright file="MixinAttributesAreInjectedIntoTarget.cs" company="Copacetic Software"> 
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
using System.Linq;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinAttributesAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
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
                    [AttributeUsage(AttributeTargets.All, Inherited = true)]
                    public class InheritedAttribute : System.Attribute{}

                    [AttributeUsage(AttributeTargets.All, Inherited = false)]
                    public class NonInheritedAttribute : System.Attribute{}

                    public class ChildOfInheritedAttribute : InheritedAttribute{}
                    public class ChildOfNonInheritedAttribute : NonInheritedAttribute{}

                    public class AttributeWithConstructor : Attribute
                    {   
                        public AttributeWithConstructor(int i){RetrieveValue = i;}

                        public int RetrieveValue{get; private set;}
                    }

                    public class AttributeWithParameter : Attribute
                    {   
                        public int Parameter {get;set;}
                    }

                    [Inherited]
                    [NonInherited]
                    [ChildOfInheritedAttribute]
                    [ChildOfNonInheritedAttribute]
                    [AttributeWithConstructor(42)]
                    [AttributeWithParameter(Parameter = 42)]
                    public class MixinWithAttributes
                    {
                        [Inherited]
                        [NonInherited]
                        [ChildOfInheritedAttribute]
                        [ChildOfNonInheritedAttribute]
                        [AttributeWithConstructor(42)]
                        [AttributeWithParameter(Parameter = 42)]
                        public void MethodWithAttributes(){}
                    }

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.MixinWithAttributes))]
                    public partial class Target{}                        
                }";
            }
        }

        protected Type TargetType;
        public override void MainSetup()
        {
            base.MainSetup();

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            Assert.True(null != targetInstance, "Failed to get Target instance");

            TargetType = targetInstance.GetType();
        }

        [Test]
        public void ClassShouldHaveInheritedAttributes()
        {
            TargetType
                .GetCustomAttributes(false)
                .Count(x => x.GetType().Name == "InheritedAttribute")
                .ShouldEqual(1);

            TargetType
                .GetCustomAttributes(false)
                .Count(x => x.GetType().Name == "ChildOfInheritedAttribute")
                .ShouldEqual(1);

            TargetType
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "AttributeWithConstructor")
               .ShouldEqual(1);

            TargetType
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "AttributeWithParameter")
               .ShouldEqual(1);
        }

        [Test]
        public void AttributeWithConstructorShouldBeInitializedCorrectly()
        {
            var instanceOfAttribute =
                TargetType
                    .GetCustomAttributes(false)
                    .FirstOrDefault(x => x.GetType().Name == "AttributeWithConstructor");

            Assert.True(null != instanceOfAttribute,
                "Failed to Load Attribute AttributeWithConstructor instance.");
                
            ReflectionHelper
                .ExecutePropertyGet<int>(
                    instanceOfAttribute,
                    "RetrieveValue")
                .ShouldEqual(42);
        }

        [Test]
        public void AttributeWithParameterShouldBeInitializedCorrectly()
        {
            var instanceOfAttribute =
                TargetType
                    .GetCustomAttributes(false)
                    .FirstOrDefault(x => x.GetType().Name == "AttributeWithParameter");

            Assert.True(null != instanceOfAttribute,
                "Failed to Load Attribute AttributeWithParameter instance.");

            ReflectionHelper
                .ExecutePropertyGet<int>(
                    instanceOfAttribute,
                    "Parameter")
                .ShouldEqual(42);
        }

        [Test]
        public void MethodShouldHaveInheritedAttributes()
        {
            TargetType
               .GetMethod("MethodWithAttributes")
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "InheritedAttribute")
               .ShouldEqual(1);

            TargetType
               .GetMethod("MethodWithAttributes")
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "ChildOfInheritedAttribute")
               .ShouldEqual(1);

            TargetType
               .GetMethod("MethodWithAttributes")
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "AttributeWithConstructor")
               .ShouldEqual(1);

            TargetType
               .GetMethod("MethodWithAttributes")
               .GetCustomAttributes(false)
               .Count(x => x.GetType().Name == "AttributeWithParameter")
               .ShouldEqual(1);
        }

        [Test]
        public void ClassShouldNotHaveNonInheritedAttributes()
        {
            TargetType
                .GetCustomAttributes(false)
                .Count(x => x.GetType().Name == "NonInheritedAttribute")
                .ShouldEqual(0);
        }

        [Test]
        public void MethodShouldNotHaveNonInheritedAttributes()
        {
            TargetType
                .GetMethod("MethodWithAttributes")
                .GetCustomAttributes(false)
                .Count(x => x.GetType().Name == "NonInheritedAttribute")
                .ShouldEqual(0);
        }
    }
}
