//----------------------------------------------------------------------- 
// <copyright file="RecipesController.cs" company="Copacetic Software"> 
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

using System.Web.Mvc;
using System.Web.Routing;
using Antlr.Runtime.Misc;
using CopaceticSoftware.pMixins.Mvc.BAL;

namespace CopaceticSoftware.pMixins.Mvc.Controllers
{
    public class RecipesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BasicMixin()
        {
            ViewBag.Title = "Basic Mixin";

            return View();
        }

        public ActionResult AbstractMixin()
        {
            ViewBag.Title = "Abstract Mixin";

            return View();
        }

        public ActionResult DependencyInjectionMixin()
        {
            ViewBag.Title = "Dependency Injection";

            return View();
        }

        public ActionResult VirtualMethodOverrides()
        {
            ViewBag.Title = "Virtual Member Overrides";

            return View();
        }

        public ActionResult SpecificMixinConstructor()
        {
            ViewBag.Title = "Specific Mixin Constructor";

            return View();
        }

        public ActionResult NonPublicNonParameterlessConstructor()
        {
            ViewBag.Title = "Non Public & Non Parameterless Constructor";

            return View();
        }

        public ActionResult CastingAndConversionOperators()
        {
            ViewBag.Title = "Conversion Operators";

            return View();
        }

        public ActionResult MixinMasks()
        {
            ViewBag.Title = "Mixin Masks";

            return View();
        }

        public ActionResult Repository()
        {
            ViewBag.Title = "The Repository";

            return View();
        }
    }
}
