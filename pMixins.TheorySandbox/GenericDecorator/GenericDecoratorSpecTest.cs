//----------------------------------------------------------------------- 
// <copyright file="GenericDecoratorSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, March 15, 2014 7:04:18 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.GenericDecorator
{
    [TestFixture]
    public class GenericDecoratorSpecTest : SpecTestBase
    {
        private GenericDecoratorSpec _spec;

        protected override void Establish_context()
        {
            _spec = new GenericDecoratorSpec();
        }

        
        [Test]
        public void Can_Call_Decorated_Method()
        {
            _spec.SomeMethod().ShouldEqual("Decorator_Child_GenericConstraint");
        }

        /// <summary>
        /// Covered in:
        ///     <see cref="MixinIsAbstractType.CanCallConcreteMethod"/>
        /// </summary>
        [Test]
        public void Can_Call_GenericConstraining_Method()
        {
            _spec.ExtraMethod().ShouldEqual("Extra Method");
        }
    }
}
