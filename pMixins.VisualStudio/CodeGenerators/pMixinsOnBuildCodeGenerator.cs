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

using System;
using System.Diagnostics;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio.CodeGenerators
{
    /// <summary>
    /// Listens for <see cref="IVisualStudioEventProxy.OnBuildBegin"/>
    /// event and regenerates the code behind for all Targets.
    /// </summary>
    public class pMixinsOnBuildCodeGenerator
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;
        private readonly ICodeGeneratorContextFactory _codeGeneratorContextFactory;
        private readonly IpMixinsCodeGeneratorResponseFileWriter _responseFileWriter;

        public pMixinsOnBuildCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioCodeGenerator visualStudioCodeGenerator, ICodeGeneratorContextFactory codeGeneratorContextFactory, IpMixinsCodeGeneratorResponseFileWriter responseFileWriter)
        {
            _visualStudioCodeGenerator = visualStudioCodeGenerator;
            _codeGeneratorContextFactory = codeGeneratorContextFactory;
            _responseFileWriter = responseFileWriter;
            
            visualStudioEventProxy.OnBuildBegin += HandleBuild;
        }

        private void HandleBuild(object sender, VisualStudioBuildEventArgs e)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                _log.InfoFormat("HandleBuild started.");

                _visualStudioCodeGenerator
                    .GenerateCode(
                        _codeGeneratorContextFactory.GenerateContext(s => s.GetValidPMixinFiles()))
                    .MapParallel(_responseFileWriter.WriteCodeGeneratorResponse);
            }
            catch (Exception exc)
            {
                _log.Error("Exception in HandleBuild", exc);
            }
            finally
            {
                _log.InfoFormat("HandleBuild Completed in [{0}] ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
