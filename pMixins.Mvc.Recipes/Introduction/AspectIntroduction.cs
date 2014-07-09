using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Interceptors;

// ReSharper disable CheckNamespace
namespace pMixins.Mvc.Recipes.Introduction.Aspect
// ReSharper restore CheckNamespace
{
    public class Aspect : MixinInterceptorBase
    {
        public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
        {
            Console.WriteLine("Before " + eventArgs.MemberName);
        }

        public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
        {
            Console.WriteLine("After " + eventArgs.MemberName);

            eventArgs.ReturnValue = eventArgs.ReturnValue + " - Now with Aspects!";
        }
    }
    
    [pMixin(Mixin = typeof(HelloWorld), 
        Interceptors = new[] { typeof(Aspect) })]
    public partial class AspectIntroduction
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new AspectIntroduction().SayHello());
        }
    }


}
