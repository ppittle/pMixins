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
using CopaceticSoftware.pMixins.Mvc.BAL;

namespace CopaceticSoftware.pMixins.Mvc.Controllers
{
    public class RecipesController : Controller
    {
        /*
        private readonly RecipeRepository _recipeRepository;

        public RecipesController()
        {
            _recipeRepository = new RecipeRepository();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        public ActionResult Index(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId))
                return View("AllRecipes", _recipeRepository.GetAllRecipes());

            var recipe = _recipeRepository.GetRecipeById(recipeId);

            if (null == recipe)
                return new HttpNotFoundResult("No recipe with Id [" + recipeId +"] was found.");

            return View("Recipe", recipe);
        }
         */

        public ActionResult BasicMixin()
        {
            return View();
        }
    }
}
