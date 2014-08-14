//----------------------------------------------------------------------- 
// <copyright file="TypeDeclarationExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 14, 2014 12:55:17 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Extensions
{
    public static class TypeDeclarationExtensions
    {
        public static IEnumerable<IMember> ResolveMembers(this TypeDeclaration type, IPipelineCommonState pipeline)
        {
            return
                pipeline.Context.TypeResolver.Resolve(type)
                    .Type.GetMembers();
        }
    }
}
