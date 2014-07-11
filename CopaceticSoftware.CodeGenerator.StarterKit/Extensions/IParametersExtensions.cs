//----------------------------------------------------------------------- 
// <copyright file="IParametersExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 5:48:10 PM</date> 
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
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class IParametersExtensions
    {
        public static IList<KeyValuePair<string, string>> ToKeyValuePair(this IEnumerable<IParameter> parameters)
        {
            if (null == parameters)
                return new List<KeyValuePair<string, string>>();

            return parameters.Select(p => new KeyValuePair<string, string>(p.Type.GetOriginalFullNameWithGlobal(), p.Name)).ToList();
        }
    }
}
