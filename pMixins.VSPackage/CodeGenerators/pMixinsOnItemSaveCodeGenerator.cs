//----------------------------------------------------------------------- 
// <copyright file="pMixinsOnItemSaveCodeGenerator.cs" company="Copacetic Software"> 
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

using System;
using System.IO;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.pMixins.VisualStudio;

namespace CopaceticSoftware.pMixins_VSPackage.CodeGenerators
{
    /// <summary>
    /// Listens for <see cref="IVisualStudioEventProxy.OnProjectItemSaved"/>
    /// events.  If the save event is for a file containing a Mixin,
    /// this class regenerates the code behind for all Targets using
    /// the Mixins
    /// </summary>
    public class pMixinsOnItemSaveCodeGenerator
    {
        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;
        private readonly IpMixinsSolutionManager _solutionManager;

        public pMixinsOnItemSaveCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioCodeGenerator visualStudioCodeGenerator, IpMixinsSolutionManager solutionManager)
        {
            _visualStudioCodeGenerator = visualStudioCodeGenerator;
            _solutionManager = solutionManager;

            visualStudioEventProxy.OnProjectItemSaved += HandleProjectItemSaved;
        }

        //TODO: Async!
        private void HandleProjectItemSaved(object sender, ProjectItemSavedEventArgs projectItemSavedEventArgs)
        {
            /*
            var impactedTargetFiles =
                _solutionManager.MixinDependencies
                    .Where(kvp => kvp.Value.Any(v => v.FileName.Equals(projectItemSavedEventArgs.ClassFullPath, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(kvp => kvp.Key);

            var updatedBits = 
                impactedTargetFiles
                    .ToDictionary(
                        csharpFile => csharpFile.FileName,

                        )

            _visualStudioCodeGenerator.GenerateCode()
             */
        }
    }
}
