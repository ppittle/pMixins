//----------------------------------------------------------------------- 
// <copyright file="Recipe.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;

namespace CopaceticSoftware.pMixins.Mvc.Models
{
    public class Recipe
    {
        public Recipe()
        {
            CodeDescriptions = new List<CodeDescriptionPair>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public IEnumerable<CodeDescriptionPair> CodeDescriptions { get; set; }
    }

    public class CodeDescriptionPair
    {
        public string Forward { get; set; }
        public string Code { get; set; }
        public string Afterward { get; set; }
    }
}
