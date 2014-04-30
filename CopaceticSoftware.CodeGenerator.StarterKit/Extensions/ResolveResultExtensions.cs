//----------------------------------------------------------------------- 
// <copyright file="ResolveResultExtensions.cs" company="Copacetic Software"> 
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
using System.Linq;
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.Semantics;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class ResolveResultExtensions
    {
        public static object GetValue(this ResolveResult result, bool ignoreError = false)
        {
            if (null == result || (result.IsError && !ignoreError))
                return null;

            object value = null;

            if (result.TryCast<UnknownIdentifierResolveResult>(r => value = r.Identifier))
                return value;

            if (result.TryCast<ConstantResolveResult>(r => value = r.ConstantValue))
                return value;

            if (result.TryCast<MemberResolveResult>(r => value = r.ConstantValue))
                return value;

            if (result.TryCast<TypeOfResolveResult>(r => value = r.ReferencedType))
                return value;

            if (result.TryCast<ConversionResolveResult>(r => value = r.Input.GetValue()))
                return value;

            if (result.TryCast<ArrayCreateResolveResult>(r =>
                                value = r.InitializerElements
                                    .Select(x => x.GetValue(ignoreError: true))
                                    .ToArray()))
                return value;

            throw new Exception("THIS IS A BUG: Do not know how to get Value from a ResolveResult: " + result.GetType().Name);
        }
    }
}
