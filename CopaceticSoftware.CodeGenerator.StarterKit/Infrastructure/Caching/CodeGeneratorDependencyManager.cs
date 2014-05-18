//----------------------------------------------------------------------- 
// <copyright file="CodeGeneratorDependencyManager.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 13, 2014 5:09:17 PM</date> 
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
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    public interface ICodeGeneratorDependencyManager
    {
        List<CSharpFile> GetFilesThatDependOn(FilePath classFileName);
    }

    public class CodeGeneratorDependencyManager : ICodeGeneratorDependencyManager
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<FilePath, CodeGeneratorDependency> _codeGeneratorDependencies =
            new ConcurrentDictionary<FilePath, CodeGeneratorDependency>();

        public CodeGeneratorDependencyManager(IVisualStudioEventProxy visualStudioEventProxy, ICodeGeneratorDependencyFactory codeGeneratorDependencyFactory)
        {
            WireUpVisualStudioEvents(visualStudioEventProxy, codeGeneratorDependencyFactory);
        }

        private void WireUpVisualStudioEvents(IVisualStudioEventProxy visualStudioEventProxy,
            ICodeGeneratorDependencyFactory codeGeneratorDependencyFactory)
        {
            CodeGeneratorDependency dummy;

            visualStudioEventProxy.OnCodeGenerated += (sender, args) =>
            {
                var dependency = codeGeneratorDependencyFactory.BuildDependency(args.Response);

                if (null != dependency)
                    //Add Dependency
                    _codeGeneratorDependencies.AddOrUpdate(
                        dependency.TargetFile.FileName,
                        dependency,
                        (s, d) => dependency);

                else if (
                    null != args.Response &&
                    null != args.Response.CodeGeneratorContext &&
                    null != args.Response.CodeGeneratorContext.Source &&
                    !args.Response.CodeGeneratorContext.Source.FileName.IsNullOrEmpty())
                    //Remove Dependency
                    _codeGeneratorDependencies.TryRemove(
                        args.Response.CodeGeneratorContext.Source.FileName,
                        out dummy);
            };

            visualStudioEventProxy.OnProjectItemRemoved += (sender, args) =>
            {
                if (_codeGeneratorDependencies.TryRemove(
                    args.ClassFullPath,
                    out dummy))
                {
                    _log.InfoFormat("Evicted [{0}]", args.ClassFullPath);
                }
            };

            visualStudioEventProxy.OnSolutionClosing +=
                (sender, args) =>
                {
                    _log.Info("Solution closing.  Clearing cache");

                    _codeGeneratorDependencies = 
                        new ConcurrentDictionary<FilePath, CodeGeneratorDependency>();
                };
        }

        public List<CSharpFile> GetFilesThatDependOn(FilePath classFileName)
        {
            return
                _codeGeneratorDependencies.Values
                    .Where(
                        d =>
                            d.FileDependencies.Any(
                                f => f.FileName.Equals(classFileName)))
                    .Select(d => d.TargetFile)
                    .ToList();
        }
    }
}
