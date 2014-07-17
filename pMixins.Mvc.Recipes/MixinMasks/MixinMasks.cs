//----------------------------------------------------------------------- 
// <copyright file="MixinMasks.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 17, 2014 11:28:20 AM</date> 
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
using CopaceticSoftware.pMixins.Attributes;
using pMixins.Mvc.Recipes.Repository;

namespace pMixins.Mvc.Recipes.MixinMasks
{
    public class InMemoryRepository<T> : ICreate<T>, IReadById<T>, IReadAll<T>, IUpdate<T>, IDelete<T>
        where T : IEntity
    {
        private readonly List<T> _dataStore = new List<T>(); 

        public bool Create(T entity)
        {
            _dataStore.Add(entity);
            return true;
        }

        public T ReadById(int id)
        {
            return _dataStore.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> ReadAll()
        {
            return _dataStore.ToArray();
        }

        public bool Update(T entity)
        {
            Delete(entity);
            Create(entity);

            return true;
        }

        public bool Delete(T entity)
        {
            _dataStore.Remove(entity);

            return true;
        }
    }

    [pMixin(Mixin = typeof(InMemoryRepository<MyEntity>), 
        Masks = new []{typeof(ICreate<MyEntity>), typeof(IReadById<MyEntity>)})]
    public partial class ExampleTestMyEntityRepository
    {
    }

    internal interface IExampleTestMyEntityRepository : ICreate<MyEntity>,
        IReadById<MyEntity>
    {
    }

    [pMixin(Mixin = typeof(InMemoryRepository<MyEntity>),
        Masks = new[] { typeof(IExampleTestMyEntityRepository) })]
    public partial class AnotherTestMyEntityRepository : IExampleTestMyEntityRepository
    {
    }
}
