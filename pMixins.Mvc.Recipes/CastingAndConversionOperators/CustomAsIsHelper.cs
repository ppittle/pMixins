//----------------------------------------------------------------------- 
// <copyright file="CustomAsIsHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 18, 2014 12:15:53 PM</date> 
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
using CopaceticSoftware.pMixins.ConversionOperators;

namespace pMixins.Mvc.Recipes.CastingAndConversionOperators
{
    public class CustomAsIsHelper : AsIsHelper.IAsIsImplementation
    {
        public T As<T>(object obj) where T : class
        {
            throw new NotImplementedException();
        }

        public bool Is<T>(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public static class CustomAsIsHelperRegister
    {
        public static void Register()
        {
            AsIsHelper.AsIsImplementationFactory = () =>
                new CustomAsIsHelper();
        }
    }
}
