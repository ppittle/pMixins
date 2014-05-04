//----------------------------------------------------------------------- 
// <copyright file="pMixinVisualStudioCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, March 8, 2014 10:44:15 PM</date> 
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
using System.Reflection;
using System.Runtime.InteropServices;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.VisualStudio;
using CopaceticSoftware.pMixins_VSPackage.Infrastructure;
using log4net;
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Microsoft.VisualStudio.Shell;
using Ninject;
using VSLangProj80;

namespace CopaceticSoftware.pMixins_VSPackage.CodeGenerators
{
    [ComVisible(true)]
    [Guid("3E3CAED9-8C24-4332-A774-059F50FF38D6")]
    [ProvideObject(typeof (pMixinsSingleFileCodeGenerator))]
    [CodeGeneratorRegistration(
        typeof (pMixinsSingleFileCodeGenerator),
        "C# pMixins Code Generator",
        vsContextGuids.vsContextGuidVCSProject,
        GeneratesDesignTimeSource = true)]
    public class pMixinsSingleFileCodeGenerator : BaseCodeGeneratorWithSite
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;

        public pMixinsSingleFileCodeGenerator()
        {
            _visualStudioCodeGenerator = ServiceLocator.Kernel.Get<IVisualStudioCodeGenerator>();

            _log.Info("pMixinsSingleFileCodeGenerator was constructed");
        }

        protected override string GetDefaultExtension()
        {
            return ".mixin.cs";
        }

        protected override byte[] GenerateCode(string inputFileContent)
        {
            return
                _visualStudioCodeGenerator.GenerateCode(
                    new[]
                    {
                        new RawSourceFile
                        {
                            FileContents = inputFileContent,
                            FileName = GetProjectItem().Name,
                            ProjectFileName = GetProject().FullName
                        }
                    })
                    .First();
        }
    }
}
