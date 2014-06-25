using CopaceticSoftware.pMixins.Attributes;

namespace pMixins.Mvc.Recipes.BasicMixinExample
{
    public class AnswerToTheUniverseMixin
    {
        public int AnswerToTheUniverse()
        {
            return 42;
        }
    }

    public class HelloWorldMixin
    {
        public string GetHelloWorld()
        {
            return "Hello World";
        }
    }

    [pMixin(Mixin = typeof(AnswerToTheUniverseMixin))]
    [pMixin(Mixin = typeof(HelloWorldMixin))]
    public partial class BasicMixinExample
    {
    }
}
