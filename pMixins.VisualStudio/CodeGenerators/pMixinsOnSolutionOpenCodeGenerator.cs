//----------------------------------------------------------------------- 
// <copyright file="pMixinsOnSolutionOpenCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 14, 2014 11:53:22 PM</date> 
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching;
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using CopaceticSoftware.CodeGenerator.StarterKit.Threading;
using CopaceticSoftware.pMixins.CodeGenerator;
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using CopaceticSoftware.pMixins.VisualStudio.IO;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio.CodeGenerators
{
    /// <summary>
    /// Listens for <see cref="IVisualStudioEventProxy.OnSolutionOpening"/>
    /// events.  On Solution Open, regenerates all Mixins, which warms up
    /// the <see cref="ICodeGeneratorDependencyManager"/>.
    /// </summary>
    /// <remarks>
    /// Should be singleton.
    /// </remarks>
    public class pMixinsOnSolutionOpenCodeGenerator
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;
        private readonly ICodeGeneratorContextFactory _codeGeneratorContextFactory;
        private readonly IpMixinsCodeGeneratorResponseFileWriter _responseFileWriter;
        private readonly ITaskFactory _taskFactory;

        public static Task OnSolutionOpeningTask = null;

        private static readonly object _lock = new object();

        public pMixinsOnSolutionOpenCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, ICodeGeneratorContextFactory codeGeneratorContextFactory, IpMixinsCodeGeneratorResponseFileWriter responseFileWriter, ITaskFactory taskFactory, IVisualStudioCodeGenerator visualStudioCodeGenerator)
        {
            _codeGeneratorContextFactory = codeGeneratorContextFactory;
            _responseFileWriter = responseFileWriter;
            _taskFactory = taskFactory;
            _visualStudioCodeGenerator = visualStudioCodeGenerator;

            visualStudioEventProxy.OnSolutionOpening += (s, a) => WarmUpCodeGeneratorDependencyManager();

            visualStudioEventProxy.OnSolutionClosing += (s, a) => OnSolutionOpeningTask = null;
        }

        private void WarmUpCodeGeneratorDependencyManager()
        {
            if (null != OnSolutionOpeningTask)
                return;

            lock (_lock)
            {
                if (null != OnSolutionOpeningTask)
                    return;

                OnSolutionOpeningTask =
                    _taskFactory.StartNew(
                        () =>
                        {
                            using (new LoggingActivity("HandleSolutionOpening"))
                                try
                                {
                                    _visualStudioCodeGenerator.GenerateCode(
                                        _codeGeneratorContextFactory
                                            .GenerateContext(s => s.GetValidPMixinFiles()))
                                   .MapParallel( _responseFileWriter.WriteCodeGeneratorResponse );
                                }
                                catch (Exception exc)
                                {
                                    _log.Error("Exception in WarmUpCodeGeneratorDependencyManager",
                                        exc);
                                }
                        });
            }
        }
    }
}
