//----------------------------------------------------------------------- 
// <copyright file="pMixinsOnBuildCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 10:52:59 PM</date> 
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
using CopaceticSoftware.pMixins.VisualStudio;

namespace CopaceticSoftware.pMixins_VSPackage.CodeGenerators
{
    /// <summary>
    /// Listens for <see cref="IVisualStudioEventProxy.OnBuildBegin"/>
    /// event and regenerates the code behind for all Targets.
    /// </summary>
    public class pMixinsOnBuildCodeGenerator
    {
        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;
        private readonly IpMixinsSolutionManager _solutionManager;

        public pMixinsOnBuildCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioCodeGenerator visualStudioCodeGenerator, IpMixinsSolutionManager solutionManager)
        {
            _visualStudioCodeGenerator = visualStudioCodeGenerator;
            _solutionManager = solutionManager;

            visualStudioEventProxy.OnBuildBegin += HandleBuild;
        }

        private void HandleBuild(object sender, VisualStudioBuildEventArgs e)
        {
            //regenerate the code for all _solutionManager.MixinDependencies
        }
    }
}
