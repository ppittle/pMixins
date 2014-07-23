//----------------------------------------------------------------------- 
// <copyright file="AddMixinClassAttributesToGeneratedClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, January 30, 2014 5:05:41 PM</date> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.TargetPartialClassGenerator
{
    public class AddMixinClassAttributesToGeneratedClass : IPipelineStep<pMixinGeneratorPipelineState>
    {
        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var doNotMixinType = typeof(DoNotMixinAttribute)
                .ToIType(manager.BaseState.CommonState.Context.TypeResolver.Compilation);

            manager.GeneratedClass.GeneratedClassSyntaxTree.Attributes.AddRange(
                    manager.CurrentpMixinAttribute.Mixin
                        .GetAttributes()
                        .FilterOutNonInheritedAttributes()
                        //Don't add a DoNotMixin Type
                        .Where(a => !a.AttributeType.Equals(doNotMixinType))
                        .ConvertToAttributeAstTypes()
                        .Select(attributeAstType => new AttributeSection(attributeAstType)));

            return true;
        }
    }
}
