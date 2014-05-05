//----------------------------------------------------------------------- 
// <copyright file="pMixinsMicrosoftBuildPRojectAssemblyReferenceResolver.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 2:54:29 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Attributes;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.Build.Evaluation;

namespace CopaceticSoftware.pMixins.VisualStudio
{
    public class pMixinsMicrosoftBuildProjectAssemblyReferenceResolver :
        MicrosoftBuildProjectAssemblyReferenceResolver
    {
        private static string pMixinsAssemblyName = typeof (pMixinAttribute).Assembly.GetName().Name;

        protected override IEnumerable<IUnresolvedAssembly> ResolveAssemblyReferences(Project project, string projectFileName)
        {
            return base.ResolveAssemblyReferences(project, projectFileName)
                //Ensure there is 1 and only 1 reference to pMixins
                .Where(r => !r.AssemblyName.Equals(pMixinsAssemblyName))
                .Union(new[] { LoadAssembly(typeof(pMixinAttribute).Assembly.Location) })
                .ToList();
        }
    }
}
