//----------------------------------------------------------------------- 
// <copyright file="pMixinsSolutionManager.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 2:27:45 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.VisualStudio
{
    public interface IpMixinsSolutionManager : ISolutionManager
    {
        /// <summary>
        /// A collection of source class files that a Target depends on.  The Key
        /// is a file with the pMixins attribute and the Value is a collection
        /// of files where the <see cref="pMixinAttribute.Mixin"/> is defined.
        /// </summary>
        IDictionary<CSharpFile, IEnumerable<CSharpFile>> MixinDependencies { get; }
    }

    public class pMixinsSolutionManager : SolutionManager, IpMixinsSolutionManager
    {
        public pMixinsSolutionManager(IVisualStudioEventProxy visualStudioEventProxy, ISolutionFactory solutionFactory) : base(visualStudioEventProxy, solutionFactory)
        {
        }

        public IDictionary<CSharpFile, IEnumerable<CSharpFile>> MixinDependencies { get; private set; }
    }
}
