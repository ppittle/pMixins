//----------------------------------------------------------------------- 
// <copyright file="ProjectExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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

using Microsoft.Build.Evaluation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class ProjectExtensions
    {
        public static bool? GetBoolProperty(this Project p, string propertyName)
        {
            string val = p.GetPropertyValue(propertyName);
            bool result;
            if (bool.TryParse(val, out result))
                return result;

            return null;
        }
    }
}
