//----------------------------------------------------------------------- 
// <copyright file="MixedInMemberAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, August 18, 2014 12:20:44 AM</date> 
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

namespace CopaceticSoftware.pMixins.Attributes
{
    /// <summary>
    /// Marker attribute used to decorate members that were added by pMixin.
    /// </summary>
    /// <remarks>
    /// Internal use only.  This attribute is used by the Code Generator internally
    /// and has no external use. 
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class MixedInMemberAttribute : Attribute 
    { }
}
