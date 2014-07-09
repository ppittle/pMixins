//----------------------------------------------------------------------- 
// <copyright file="AspectIntroduction.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 9, 2014 10:52:31 PM</date> 
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
