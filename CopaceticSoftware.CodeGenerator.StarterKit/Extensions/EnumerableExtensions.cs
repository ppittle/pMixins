//----------------------------------------------------------------------- 
// <copyright file="EnumerableExtensions.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Map<T>(this IEnumerable<T> collection, Action<T> func)
        {
            if (null == collection)
                return;

            if (null == func)
                return;

            foreach (var item in collection)
                func(item);
        }

        public static void MapParallel<T>(this IEnumerable<T> collection, Action<T> func)
        {
            if (null == collection)
                return;

            if (null == func)
                return;

            Parallel.ForEach(collection, func);
        }

        //http://stackoverflow.com/questions/2471588/how-to-get-index-using-linq
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (null == collection)
                throw new ArgumentNullException("collection");

            return collection
                .Select((x, i) => new {item = x, index = i})
                .First(x => predicate(x.item))
                .index;
        }
    }
}
