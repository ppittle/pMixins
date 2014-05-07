//----------------------------------------------------------------------- 
// <copyright file="KernelFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 11:56:10 AM</date> 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Ninject;
using CopaceticSoftware.pMixins.VisualStudio.Ninject;
using Ninject;

namespace CopaceticSoftware.pMixins.Tests.Common
{
    public static class KernelFactory
    {
        public static IKernel BuildDefaultKernelForTests()
        {
            var Kernel =  
                new StandardKernel(
                    new StandardModule(),
                    new pMixinsStandardModule());

            Kernel.Rebind<IVisualStudioEventProxy>().To<TestVisualStudioEventProxy>();
            Kernel.Rebind<IVisualStudioWriter>().To<TestVisualStudioWriter>();
            Kernel.Rebind<IMicrosoftBuildProjectAssemblyReferenceResolver>()
                .To<TestMicrosoftBuildProjectAssemblyReferenceResolver>().InSingletonScope();

            return Kernel;
        }
    }
}
