//----------------------------------------------------------------------- 
// <copyright file="CSharpProjectFactory.cs" company="Copacetic Software"> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface ICSharpProjectFactory
    {
        CSharpProject BuildProject(FilePath projectFileName, string title);
    }

    public class CSharpProjectFactory : ICSharpProjectFactory
    {
        private readonly IMicrosoftBuildProjectAssemblyReferenceResolver _assemblyReferenceResolver;
        private readonly ICSharpFileFactory _cSharpFileFactory;
        private readonly IMicrosoftBuildProjectLoader _microsoftBuildProjectLoader;

        public CSharpProjectFactory(IMicrosoftBuildProjectAssemblyReferenceResolver assemblyReferenceResolver, ICSharpFileFactory cSharpFileFactory, IMicrosoftBuildProjectLoader microsoftBuildProjectLoader)
        {
            _assemblyReferenceResolver = assemblyReferenceResolver;
            _cSharpFileFactory = cSharpFileFactory;
            _microsoftBuildProjectLoader = microsoftBuildProjectLoader;
        }

        public CSharpProject BuildProject(FilePath projectFileName, string title)
        {
            return new CSharpProject(
                _cSharpFileFactory,
                new MicrosoftBuildProject(_microsoftBuildProjectLoader, _assemblyReferenceResolver, projectFileName),
                title);
        }
    }
}
