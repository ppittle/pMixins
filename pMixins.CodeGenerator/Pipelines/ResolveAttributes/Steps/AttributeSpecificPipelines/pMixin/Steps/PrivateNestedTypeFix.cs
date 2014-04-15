//----------------------------------------------------------------------- 
// <copyright file="PrivateNestedTypeFix.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 28, 2014 2:01:57 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps.AttributeSpecificPipelines.pMixin.Steps
{
    public class PrivateNestedTypeFix : IPipelineStep<ResolvepMixinAttributePipelineState>
    {
        public bool PerformTask(ResolvepMixinAttributePipelineState manager)
        {
            var resolvedClassDefs =
                manager.BaseState.Context.Source.SyntaxTree.GetClassDefinitions()
                    .Select(classDef => manager.BaseState.Context.TypeResolver.Resolve(classDef))
                    .Where(resolvedClassDef => !resolvedClassDef.IsError)
                    .ToList();

            
            if (null != manager.ResolvedResult.Mixin)
                manager.ResolvedResult.Mixin = TryResolvePrivateNestedType(manager.ResolvedResult.Mixin, resolvedClassDefs);

            for (int i = 0; i < manager.ResolvedResult.Interceptors.Count(); i++)
            {
                manager.ResolvedResult.Interceptors[i] =
                    TryResolvePrivateNestedType(manager.ResolvedResult.Interceptors[i], resolvedClassDefs);
            }

            for (int i = 0; i < manager.ResolvedResult.Masks.Count(); i++)
            {
                manager.ResolvedResult.Masks[i] =
                    TryResolvePrivateNestedType(manager.ResolvedResult.Masks[i], resolvedClassDefs);
            }

            return true;
        }

        private IType TryResolvePrivateNestedType(IType type, IEnumerable<ResolveResult> resolvedClassDefs)
        {
            if (!type.IsUnkown())
                return type;

            foreach (var resolvedClassDef in resolvedClassDefs)
            {
                if (type.FullName == resolvedClassDef.Type.Name)
                    return resolvedClassDef.Type;
            }

            return type;
        }
    }
}
