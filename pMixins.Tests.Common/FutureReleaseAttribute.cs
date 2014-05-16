//----------------------------------------------------------------------- 
// <copyright file="FutureReleaseAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, May 16, 2014 11:15:43 AM</date> 
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

namespace CopaceticSoftware.pMixins.Tests.Common
{
    public class FutureReleaseAttribute : IgnoreAttribute
    {
        public FutureReleaseAttribute() : base("Future Release") { }
    }
}
