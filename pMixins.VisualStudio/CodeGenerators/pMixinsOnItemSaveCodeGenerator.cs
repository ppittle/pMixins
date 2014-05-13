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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using CopaceticSoftware.CodeGenerator.StarterKit.Threading;
using CopaceticSoftware.pMixins.CodeGenerator;
using CopaceticSoftware.pMixins.VisualStudio.Extensions;
using CopaceticSoftware.pMixins.VisualStudio.Infrastructure;
using CopaceticSoftware.pMixins.VisualStudio.IO;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio.CodeGenerators
{
    /// <summary>
    /// Listens for <see cref="IVisualStudioEventProxy.OnProjectItemSaved"/>
    /// events.  If the save event is for a file containing a Mixin,
    /// this class regenerates the code behind for all Targets using
    /// the Mixins
    /// </summary>
    /// <remarks>
    /// Should be singleton.
    /// </remarks>
    public class pMixinsOnItemSaveCodeGenerator
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioCodeGenerator _visualStudioCodeGenerator;
        private readonly ICodeGeneratorContextFactory _codeGeneratorContextFactory;
        private readonly IpMixinsCodeGeneratorResponseFileWriter _responseFileWriter;
        private readonly ITaskFactory _taskFactory;
        
        private static ConcurrentDictionary<string, MixinDependency> _pMixinDependencies =
            new ConcurrentDictionary<string, MixinDependency>();

        private Task OnSolutionOpeningTask;
        
        public pMixinsOnItemSaveCodeGenerator(IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioCodeGenerator visualStudioCodeGenerator, ICodeGeneratorContextFactory codeGeneratorContextFactory, IpMixinsCodeGeneratorResponseFileWriter responseFileWriter, ITaskFactory taskFactory)
        {
            _visualStudioCodeGenerator = visualStudioCodeGenerator;
            _codeGeneratorContextFactory = codeGeneratorContextFactory;
            _responseFileWriter = responseFileWriter;
            _taskFactory = taskFactory;

            WireUpVisualStudioProxyEvents(visualStudioEventProxy);
        }

        private void WireUpVisualStudioProxyEvents(IVisualStudioEventProxy visualStudioEventProxy)
        {
            visualStudioEventProxy.OnSolutionOpening += HandleSolutionOpening;

            visualStudioEventProxy.OnProjectItemSaveComplete += HandleProjectItemSaveComplete;

            visualStudioEventProxy.OnCodeGenerated += HandleOnCodeGenerated;

            visualStudioEventProxy.OnProjectItemRemoved +=
                (sender, args) =>
                {
                    MixinDependency dummy;

                    if (_pMixinDependencies.TryRemove(args.ClassFullPath, out dummy))
                        _log.InfoFormat("Evicted [{0}]", args.ClassFullPath);
                };

            visualStudioEventProxy.OnSolutionClosing +=
                (sender, args) =>
                {
                    _log.Info("Solution closing.  Clearing cache");
                    _pMixinDependencies = new ConcurrentDictionary<string, MixinDependency>();
                };
        }
        
        private void HandleOnCodeGenerated(object sender, CodeGeneratedEventArgs args)
        {
            var response = args.Response as pMixinPartialCodeGeneratorResponse;

            if (null == response)
                return;

            _pMixinDependencies.AddOrUpdate(
                response.CodeGeneratorContext.Source.FileName,
                f => new MixinDependency(response),
                (f, m) => new MixinDependency(response));
        }

        private void HandleSolutionOpening(object sender, EventArgs e)
        {
            OnSolutionOpeningTask = 
                _taskFactory.StartNew(() =>
            {
                using (var activity = new LoggingActivity("HandleSolutionOpening"))
                try
                {
                    var generator = new pMixinPartialCodeGenerator();

                    _codeGeneratorContextFactory
                        //Load up all PMixin Files
                        .GenerateContext(s => s.GetValidPMixinFiles())
                        //Process each file in parallel
                        .AsParallel()
                        //Run the Generate Code Pipeline
                        .Select(context => generator.GeneratePartialCode(context))
                        //Process the Response
                        .Map(respose => HandleOnCodeGenerated(this, new CodeGeneratedEventArgs {Response = respose}));
                }
                catch (Exception exc)
                {
                    _log.Error("Exception in HandleSolutionOpening", exc);
                }
            });
        }

        private void HandleProjectItemSaveComplete(object sender, ProjectItemSavedEventArgs args)
        {
            _taskFactory.StartNew(() =>
            {
                OnSolutionOpeningTask.Wait();

                using (var activity = new LoggingActivity("HandleProjectItemSaved"))
                try
                {
                    //Generate code for the file saved
                    _visualStudioCodeGenerator
                        .GenerateCode(
                            _codeGeneratorContextFactory.GenerateContext(
                                s => s.GetValidPMixinFiles().Where(f => f.FileName.Equals(args.ClassFullPath))))
                        .Map(_responseFileWriter.WriteCodeGeneratorResponse);

                    //Generate code for dependencies
                    var filesToUpdate =
                        _pMixinDependencies.Values
                            .Where(d => 
                                d.FileDependencies.Any(
                                    f => f.FileName.Equals(args.ClassFullPath, StringComparison.InvariantCultureIgnoreCase)))
                            .Select(d => d.TargetFile)
                            .ToList();

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Will update [{0}]",
                            string.Join(Environment.NewLine,
                                filesToUpdate.Select(x => x.FileName)));

                        _visualStudioCodeGenerator
                        .GenerateCode(
                            _codeGeneratorContextFactory.GenerateContext(filesToUpdate))
                        .MapParallel(_responseFileWriter.WriteCodeGeneratorResponse);
                }
                catch (Exception exc)
                {
                    _log.Error("Exception in HandleProjectItemSaved", exc);
                }
            });
        }
    }
}
