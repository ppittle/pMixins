//----------------------------------------------------------------------- 
// <copyright file="BasicMixinExampleTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, June 25, 2014 7:00:33 PM</date> 
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

namespace pMixins.Mvc.Recipes.BasicMixin
{
    [TestFixture]
    public class BasicMixinTest 
    {
        [Test]
        public void CanCallHelloWorld()
        { 
            Assert.AreEqual(
                new BasicMixinExample().GetHelloWorld(),
                "Hello World");
        }

        [Test]
        public void CanGetAnswerToTheUniverse()
        {
            Assert.AreEqual(
               new BasicMixinExample().AnswerToTheUniverse(),
               42);
        }

        [Test]
        public void CanDoImplicitConversion()
        {
            HelloWorldMixin helloWorld = new BasicMixinExample();

            Assert.AreEqual(
                helloWorld.GetHelloWorld(),
                "Hello World");
        }

        [Test]
        public void CanDoExplicitConversion()
        {
            var answerToTheUniverse = (AnswerToTheUniverseMixin) new BasicMixinExample();

            Assert.AreEqual(
               answerToTheUniverse.AnswerToTheUniverse(),
               42);
        }
    }
}
