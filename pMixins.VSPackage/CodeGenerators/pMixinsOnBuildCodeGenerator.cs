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

using System.IO;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
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
        private readonly IVisualStudioProjectHelper _visualStudioProjectHelper;

        public pMixinsOnBuildCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioCodeGenerator visualStudioCodeGenerator, IpMixinsSolutionManager solutionManager, IVisualStudioProjectHelper visualStudioProjectHelper)
        {
            _visualStudioCodeGenerator = visualStudioCodeGenerator;
            _solutionManager = solutionManager;
            _visualStudioProjectHelper = visualStudioProjectHelper;

            visualStudioEventProxy.OnBuildBegin += HandleBuild;
        }

        private void HandleBuild(object sender, VisualStudioBuildEventArgs e)
        {
            /*
             _solutionManager.CodeGeneratedFiles
                 .Where(f => File.Exists(f.FileName))
                 .Map(f => _visualStudioProjectHelper.SaveFile(f.Project.FileName, f.FileName));
            */
            
            _visualStudioCodeGenerator.GenerateCode(
                _solutionManager.CodeGeneratedFiles.Select(
                    f => new RawSourceFile
                         {
                             FileName = f.FileName,
                             FileContents = File.ReadAllText(f.FileName),
                             ProjectFileName = f.Project.FileName
                         }))
                .Map(WriteMixinFileAndAddToProject);
            
        }

        private void WriteMixinFileAndAddToProject(CodeGeneratorResponse response)
        {
            if (null == response.CodeGeneratorContext)
                return;

            var filepath = 
                Path.Combine(
                    Path.GetDirectoryName(response.CodeGeneratorContext.Source.FileName) ?? "",
                    Path.GetFileNameWithoutExtension(response.CodeGeneratorContext.Source.FileName) ?? "")
                + pMixinsSingleFileCodeGenerator.DefaultExtension;

            if (File.Exists(filepath))
                File.Delete(filepath);

            File.WriteAllText(filepath, response.GeneratedCodeSyntaxTree.GetText());

            /*
            _visualStudioProjectHelper.RegisterCodeGeneratedFile(
                response.CodeGeneratorContext.Source.Project.FileName,
                response.CodeGeneratorContext.Source.FileName,
                filepath);
             */
        }
    }
}
