//----------------------------------------------------------------------- 
// <copyright file="pMixinsStandardModule.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 2:37:07 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.CodeGenerator;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using CopaceticSoftware.pMixins.VisualStudio.Infrastructure;
using CopaceticSoftware.pMixins.VisualStudio.IO;
using Ninject.Modules;

namespace CopaceticSoftware.pMixins.VisualStudio.Ninject
{
    public class pMixinsStandardModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPartialCodeGenerator>().To<pMixinPartialCodeGenerator>();
            
            Rebind<IMicrosoftBuildProjectAssemblyReferenceResolver>().To<pMixinsMicrosoftBuildProjectAssemblyReferenceResolver>().InSingletonScope();

            Bind<IVisualStudioCodeGenerator>().To<VisualStudioCodeGenerator>();

            Bind<IpMixinsCodeGeneratorResponseFileWriter>().To<pMixinsCodeGeneratorResponseFileWriter>();
        }
    }
}
