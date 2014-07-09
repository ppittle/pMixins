using System;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Interceptors;

namespace pMixins.Mvc.Recipes.Introduction
{
    public class HelloWorld
    {
        public string SayHello()
        {
            Console.WriteLine("Saying Hello");
            return "Hello World!";
        }
    }

    [pMixin(Mixin = typeof(HelloWorld))]
    public partial class Introduction
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new Introduction().SayHello());
        }
    }
    
}
