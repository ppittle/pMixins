//----------------------------------------------------------------------- 
// <copyright file="ConcurrentSpecializedList.cs" company="Copacetic Software"> 
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

using System.Collections.Generic;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Collections
{
    public class ConcurrentSpecializedList<T>
    {
        private static object _lock = new object();

        private readonly List<T> _backingList = new List<T>();
        public void Add(T item)
        {
            lock(_lock)
                _backingList.Add(item);
        }

        public T[] CopyToArrayAndClear()
        {
            T[] buffer;

            lock (_lock)
            {
                buffer = _backingList.ToArray();
                _backingList.Clear();
            }

            return buffer;
        }

        public void Clear()
        {
            lock(_lock)
                _backingList.Clear();
        }
    }
}
