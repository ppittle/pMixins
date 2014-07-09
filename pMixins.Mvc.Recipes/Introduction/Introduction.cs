//----------------------------------------------------------------------- 
// <copyright file="Introduction.cs" company="Copacetic Software"> 
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
