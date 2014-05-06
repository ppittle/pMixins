//----------------------------------------------------------------------- 
// <copyright file="ConcurrentList.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 2:54:59 PM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Collections
{
    public class ConcurrentList<T>
    {
        private static readonly object _lock = new object();

        private IList<T> _list = new List<T>();

        public void Add(T item)
        {
            lock (_lock)
                _list.Add(item);
        }

        public bool Remove(T item)
        {
            lock (_lock)
                return _list.Remove(item);
        }

        public void AddOrUpdate(Func<T, bool> matchFunc, T item)
        {
            lock (_lock)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (matchFunc(_list[i]))
                    {
                        _list[i] = item;
                        return;
                    }
                }

                _list.Add(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
                return _list.Contains(item);
        }
    }
}
