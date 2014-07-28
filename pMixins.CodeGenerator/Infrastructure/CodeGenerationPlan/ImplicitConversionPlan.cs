//----------------------------------------------------------------------- 
// <copyright file="ImplicitConversionPlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, July 28, 2014 8:50:07 PM</date> 
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

using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan
{
    public class ImplicitConversionPlan
    {
        /// <summary>
        /// The <see cref="IType"/> target for this 
        /// conversion.  Can be the 
        /// <see cref="Infrastructure.CodeGenerationPlan.MixinGenerationPlan.MixinAttribute"/>
        /// itself or one of the Mixin's base classes.
        /// </summary>
        public IType ConversionTargetType { get; set; }

        /// <summary>
        /// The <see cref="Infrastructure.CodeGenerationPlan.MixinGenerationPlan"/>
        /// this <see cref="ImplicitConversionPlan"/> was generated for.
        /// </summary>
        public MixinGenerationPlan MixinGenerationPlan { get; set; }
    }
}
