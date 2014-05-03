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
using System.Reflection;
using System.Runtime.InteropServices;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Steps;
using CopaceticSoftware.pMixins_VSPackage.CodeGenerators;
using CopaceticSoftware.pMixins_VSPackage.Infrastructure;
using EnvDTE;
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

        private VisualStudioWriter _visualStudioWriter;
        private ISolutionManager _solutionManager;

        private pMixinsOnBuildCodeGenerator _onBuildCodeGenerator;
        private pMixinsOnItemSaveCodeGenerator _onItemSaveCodeGenerator;

        protected override void Initialize()
        {
            base.Initialize();

            //Get copy of current DTE
            var dte = (DTE) GetService(typeof (DTE));

            //Create a Visual Studio Writer
            _visualStudioWriter = new VisualStudioWriter(dte, this);

            //Initialize Logging
            Log4NetInitializer.Initialize(_visualStudioWriter);

            //Get a logger for this class
            _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            //Initialize the IoC Kernel
            ServiceLocator.Initialize(_visualStudioWriter);

            _log.Info("Initialized Kernel");

            InitializeSolutionManager(dte);

            InitializeFileGenerators();
        }

        private void InitializeSolutionManager(DTE dte)
        {
            _solutionManager = ServiceLocator.Kernel.Get<ISolutionManager>();

            if (null == dte.Solution)
                _log.Error("Failed to load Solution object from DTE");
            else
                _solutionManager.LoadSolution(dte.Solution.FileName);
        }

        private void InitializeFileGenerators()
        {
            try
            {
                _onBuildCodeGenerator = ServiceLocator.Kernel.Get<pMixinsOnBuildCodeGenerator>();

                _onItemSaveCodeGenerator = ServiceLocator.Kernel.Get<pMixinsOnItemSaveCodeGenerator>();
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

                if (null != _solutionManager)
                    _solutionManager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
