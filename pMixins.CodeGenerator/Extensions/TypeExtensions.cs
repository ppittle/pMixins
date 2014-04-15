//----------------------------------------------------------------------- 
// <copyright file="TypeExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, February 26, 2014 8:18:17 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Infrastructure;
using CopaceticSoftware.pMixins.Infrastructure;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Extensions
{
    public static class TypeExtensions
    {
        public static string GenerateActivationExpression(this IType type, params string[] constructorArgs)
        {
            Ensure.ArgumentNotNull(type, "type");

            return GenerateActivationExpression(type.GetOriginalFullNameWithGlobal(), constructorArgs);
        }

        public static string GenerateActivationExpression(string typeFullName, params string[] constructorArgs)
        {
            constructorArgs = constructorArgs ?? new string[0];

            return string.Format(
                "global::{0}.GetCurrentActivator().CreateInstance<{1}>({2})",
                    typeof(MixinActivatorFactory).FullName,
                    typeFullName,
                    string.Join(",", constructorArgs));
        }
    }
}
