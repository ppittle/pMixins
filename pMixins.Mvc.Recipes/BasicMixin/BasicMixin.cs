//----------------------------------------------------------------------- 
// <copyright file="BasicMixinExample.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.pMixins.Attributes;

namespace pMixins.Mvc.Recipes.BasicMixin
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

    [pMixin(Mixin = typeof(HelloWorldMixin))]
    public partial class Target { }

    [pMixin(Mixin = typeof(AnswerToTheUniverseMixin))]
    [pMixin(Mixin = typeof(HelloWorldMixin))]
    public partial class BasicMixinExample
    {
    }
}
