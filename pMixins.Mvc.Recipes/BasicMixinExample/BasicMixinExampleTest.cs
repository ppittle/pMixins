using NUnit.Framework;

namespace pMixins.Mvc.Recipes.BasicMixinExample
{
    [TestFixture]
    public class BasicMixinExampleTest 
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