//----------------------------------------------------------------------- 
// <copyright file="Guids.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, November 4, 2013 10:18:40 PM</date> 
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

// Guids.cs
// MUST match guids.h

using System;

namespace CopaceticSoftware.pMixins_VSPackage
{
    static class GuidList
    {
        public const string guidpMixin_VSPackagePkgString = "8d5593a2-b006-4306-884b-bfd9bd3ebd75";
        public const string guidpMixin_VSPackageCmdSetString = "95058db1-2ca3-47a9-b1e1-12e34c5e89e0";

        public static readonly Guid guidpMixin_VSPackageCmdSet = new Guid(guidpMixin_VSPackageCmdSetString);
    };
}
