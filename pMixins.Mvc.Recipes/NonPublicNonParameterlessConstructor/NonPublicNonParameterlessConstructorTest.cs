//----------------------------------------------------------------------- 
// <copyright file="NonPublicNonParameterlessConstructorTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 10, 2014 1:24:56 PM</date> 
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

namespace pMixins.Mvc.Recipes.NonPublicNonParameterlessConstructor
{
    [TestFixture]
    public class NonPublicNonParameterlessConstructorTest
    {
        [Test]
        public void NonAbstractMixin()
        {
            Assert.AreEqual(1, new NonPublicNonParameterlessConstructor().FavoriteNumber);
        }

        [Test]
        public void AbstractMixin()
        {
            Assert.AreEqual(1, new NonPublicNonParameterlessConstructor().AbstractFavoriteNumber);
        }
    }
}
