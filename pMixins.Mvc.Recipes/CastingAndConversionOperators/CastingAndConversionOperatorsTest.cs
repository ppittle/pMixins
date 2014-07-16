//----------------------------------------------------------------------- 
// <copyright file="ConversionOperatorsTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 16, 2014 6:38:24 PM</date> 
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

using CopaceticSoftware.pMixins.ConversionOperators;
using NUnit.Framework;

namespace pMixins.Mvc.Recipes.CastingAndConversionOperators
{
    [TestFixture]
    public class CastingAndConversionOperatorsTest
    {
        [Test]
        public void ImplicitConversion()
        {
            ISomeInterface i = new CastingAndConversionOperators();

            Assert.NotNull(i);

            SomeBaseClass c = new CastingAndConversionOperators();

            Assert.NotNull(c);
        }

        [Test]
        public void ExplicitConversion()
        {
            Assert.NotNull((ISomeInterface)new CastingAndConversionOperators());

            Assert.NotNull((SomeBaseClass)new CastingAndConversionOperators());
        }

        [Test]
        public void IsOperator()
        {
            var target = new CastingAndConversionOperators();

            //Example of 'is' in action
            Assert.True(((object)new Mixin()) is SomeBaseClass);

            //Unfortunately, there's no way to do this with classes:
            //http://stackoverflow.com/questions/18390664/c-sharp-implicit-conversion-operator-and-is-as-operator
            Assert.False(target is SomeBaseClass);

            Assert.True(target.Is<SomeBaseClass>());

            //Works on interfaces just fine
            Assert.True(target is ISomeInterface);
        }

        [Test]
        public void AsOperator()
        {
            var target = new CastingAndConversionOperators();

            //Example of 'as' in action
            Assert.NotNull(((object)new Mixin()) as SomeBaseClass);

            //Unfortunately, there's no way to do this with classes:
            //http://stackoverflow.com/questions/18390664/c-sharp-implicit-conversion-operator-and-is-as-operator
            Assert.Null(((object)target) as SomeBaseClass);

            Assert.NotNull(target.As<SomeBaseClass>());

            //Works on interfaces just fine
            Assert.NotNull(target as ISomeInterface);
        }
    }
}
