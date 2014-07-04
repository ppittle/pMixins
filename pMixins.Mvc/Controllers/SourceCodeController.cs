//----------------------------------------------------------------------- 
// <copyright file="SourceCodeController.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 3, 2014 2:25:32 PM</date> 
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
using CopaceticSoftware.pMixins.Mvc.BAL;

namespace CopaceticSoftware.pMixins.Mvc.Controllers
{
    public class SourceCodeController : Controller
    {
        private readonly ISourceCodeRepository _sourceCodeRepository;

        public SourceCodeController()
        {
            _sourceCodeRepository = new SourceCodeRepository();
        }

        // GET: SourceCode
        public ActionResult SourceCode(string file, string classname)
        {
            return PartialView(
                "_SourceCode",
                _sourceCodeRepository.GetSourceCodeForFile(file, classname));
        }
    }
}
