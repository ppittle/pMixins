//----------------------------------------------------------------------- 
// <copyright file="MicrosoftBuildProjectAssemblyReferenceResolver.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using Microsoft.Build.Evaluation;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface IMicrosoftBuildProjectAssemblyReferenceResolver
    {
        IEnumerable<IUnresolvedAssembly> ResolveAssemblyReferences(Project project);
    }

    //Should be singleton
    public class MicrosoftBuildProjectAssemblyReferenceResolver : IMicrosoftBuildProjectAssemblyReferenceResolver
    {
        private readonly ConcurrentDictionary<string, IUnresolvedAssembly> _assemblyDict;

        public MicrosoftBuildProjectAssemblyReferenceResolver()
        {
            _assemblyDict = 
                new ConcurrentDictionary<string, IUnresolvedAssembly>(Platform.FileNameComparer);
        }


        public IEnumerable<IUnresolvedAssembly> ResolveAssemblyReferences(Project project)
        {
            throw new System.NotImplementedException();
        }

        protected IUnresolvedAssembly LoadAssembly(string assemblyFileName)
        {
            return _assemblyDict.GetOrAdd(
                assemblyFileName, file => new CecilLoader().LoadAssemblyFile(file));
        }
    }
}
