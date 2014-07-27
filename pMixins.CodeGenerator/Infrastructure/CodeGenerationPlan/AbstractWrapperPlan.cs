//----------------------------------------------------------------------- 
// <copyright file="AbstractWrapperPlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, July 26, 2014 12:21:54 PM</date> 
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
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan
{
    public class AbstractWrapperPlan
    {
        /// <summary>
        /// Indicates if an Abstract Wrapper should be built
        /// for the given Mixin.
        /// </summary>
        public bool GenrateAbstractWrapper { get; set; }

        /// <summary>
        /// Indicates if all constructors should be wrapped by the 
        /// Abstract Wrapper, or if the Abstract Wrapper should have a 
        /// 'simple constructor'.  This is dictated by
        /// <see cref="pMixinAttribute.ExplicitlyInitializeMixin"/>
        /// or if <see cref="pMixinAttribute.Mixin"/> has a 
        /// parameterless constructor.
        /// </summary>
        /// <remarks>
        /// <code>
        /// <![CDATA[
        ///  var wrapAllConstructors =
        ///     manager.MixinResolvedResult.ExplicitlyInitializeMixin ||
        ///     !manager.MixinResolvedResult.Mixin.HasParameterlessConstructor();
        /// ]]>
        /// </code>
        /// </remarks>
        public bool WrapAllConstructors { get; set; }

        public IEnumerable<MemberWrapper> Members { get; set; }

        /// <summary>
        /// <c>True</c> to generate the Abstract Wrapper in an 
        /// external namespace.  <c>False</c> to generate it inside
        /// of the Target.  If the Mixin is private, wrappers must be created
        /// inside the Target.
        /// </summary>
        public bool GenerateAbstractWrapperInExternalNamespace { get; set; }

        public string AbstractWrapperClassName { get; set; }
    }
}
