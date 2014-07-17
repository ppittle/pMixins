//----------------------------------------------------------------------- 
// <copyright file="MixinMasksTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 17, 2014 12:57:54 PM</date> 
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

using NUnit.Framework;
using pMixins.Mvc.Recipes.Repository;

namespace pMixins.Mvc.Recipes.MixinMasks
{
    [TestFixture]
    public class MixinMasksTest
    {
        private readonly ExampleTestMyEntityRepository _repository = 
            new ExampleTestMyEntityRepository();

        [Test]
        public void CanWorkWithRepository()
        {
            const int entityId = 5;

            //Add item to repository
            _repository.Create(new MyEntity {Id = entityId});

            //Read item back
            Assert.NotNull(_repository.ReadById(entityId));
        }

        [Test]
        public void RepositoryDoesNotImplementIUpdate()
        {
            Assert.Null(_repository as IUpdate<MyEntity>);
        }
    }
}
