//----------------------------------------------------------------------- 
// <copyright file="pMixinsVisualStudioAutoLoader.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 3:39:23 PM</date> 
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
using System.Runtime.InteropServices;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using CopaceticSoftware.pMixins.VisualStudio.IO;
using CopaceticSoftware.pMixins.VisualStudio.Ninject;
using CopaceticSoftware.pMixins_VSPackage.Infrastructure;
using EnvDTE;
using EnvDTE80;
using log4net;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;

namespace CopaceticSoftware.pMixins_VSPackage
{
    [ComVisible(true)]
    [Guid("B77F7C65-0F9F-422A-A897-C06FDAEC9603")]
    [ProvideObject(typeof(pMixinsVisualStudioCodeGenerateInitializer))]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public class pMixinsVisualStudioCodeGenerateInitializer : Package
    {
        private ILog _log;

        private IVisualStudioWriter _visualStudioWriter;
        private ISolutionContext _solutionContext;
        private IVisualStudioEventProxy _visualStudioEventProxy;
        private ICodeBehindFileHelper _codeBehindFileHelper;
        private ISolutionFileReader _dteSolutionFileReader;

        //Keep references to code generators so they don't get garbage collected
        // ReSharper disable NotAccessedField.Local
        private pMixinsOnSolutionOpenCodeGenerator _onSolutionOpenCodeGenerator;
        private pMixinsOnBuildCodeGenerator _onBuildCodeGenerator;
        private pMixinsOnItemSaveCodeGenerator _onItemSaveCodeGenerator;

        #if DEBUG
        private VisualStudioEventProxyLogger _eventProxyLogger;
        #endif

        // ReSharper restore NotAccessedField.Local

        protected override void Initialize()
        {
            var sw = Stopwatch.StartNew();

            base.Initialize();

            //Get copy of current DTE
            var dte = (DTE) GetService(typeof (DTE));
            var dte2 = dte as DTE2;
            
            //Create a Visual Studio Writer
            _visualStudioWriter = new VisualStudioWriter(dte, this);

            _visualStudioWriter.WriteToStatusBar("Initializing pMixins Code Generator Plug-In");

            //Initialize Logging
            Log4NetInitializer.Initialize(_visualStudioWriter, this);

            //Get a logger for this class
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            _log.Info("Initialized Logging");

            //create Visual Studio Event Proxy
            _visualStudioEventProxy = new VisualStudioEventProxy(dte2);

            //Create CodeBehindFileHelper
            _codeBehindFileHelper = new CodeBehindFileHelper(dte2, typeof(pMixinsEmptySingleFileCodeGenerator).Name);

            _dteSolutionFileReader = new SolutionDTEReader(dte2);

            //Initialize the IoC Kernel
            ServiceLocator.Initialize(_visualStudioWriter, _visualStudioEventProxy, _codeBehindFileHelper, _dteSolutionFileReader);

            _log.Info("Initialized Kernel");

            InitializeSolutionContext(dte);

            InitializeFileGenerators();

            #if DEBUG
            _eventProxyLogger = ServiceLocator.Kernel.Get<VisualStudioEventProxyLogger>();
            #endif

            _visualStudioWriter.WriteToStatusBar("Initializing pMixins Code Generator Plug-In ... Complete");

            _log.InfoFormat("Initialization Complete in [{0}] ms", sw.ElapsedMilliseconds);
        }

        private void InitializeSolutionContext(DTE dte)
        {
            try
            {
                _solutionContext = ServiceLocator.Kernel.Get<ISolutionContext>();

                if (null == dte.Solution)
                    _log.Error("Failed to load Solution object from DTE");
                else if (string.IsNullOrEmpty(dte.Solution.FileName))
                    _log.Warn("dte.Solution.FileName is null or empty");
                else
                {
                    _solutionContext.SolutionFileName = new FilePath(dte.Solution.FileName);

                    _log.InfoFormat("Set Solution Context to [{0}]", dte.Solution.FileName);
                }

                _visualStudioEventProxy.OnSolutionOpening += (o, e) =>
                {
                    if (string.IsNullOrEmpty(dte.Solution.FileName))
                        _log.Warn("dte.Solution.FileName is null or empty");
                    else
                    {
                        _solutionContext.SolutionFileName = new FilePath(dte.Solution.FileName);

                        _log.InfoFormat("Set Solution Context to [{0}]", dte.Solution.FileName);
                    }
                };
            }
            catch (Exception e)
            {
                _log.Fatal("Exception initializing Solution Context", e);

                throw;
            }
        }

        private void InitializeFileGenerators()
        {
            try
            {
                _onSolutionOpenCodeGenerator = ServiceLocator.Kernel.Get<pMixinsOnSolutionOpenCodeGenerator>();

                _onBuildCodeGenerator = ServiceLocator.Kernel.Get<pMixinsOnBuildCodeGenerator>();

                _onItemSaveCodeGenerator = ServiceLocator.Kernel.Get<pMixinsOnItemSaveCodeGenerator>();

                _log.Info("Loaded Code Generators");
            }
            catch (Exception e)
            {
                _log.Fatal("Exception creating Code Generators", e);

                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != _visualStudioWriter)
                    _visualStudioWriter.Dispose();

                if (null != _visualStudioEventProxy)
                    _visualStudioEventProxy.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
