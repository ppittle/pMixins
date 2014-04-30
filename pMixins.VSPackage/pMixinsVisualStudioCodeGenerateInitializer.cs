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
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using EnvDTE;
using EnvDTE80;
using log4net;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CopaceticSoftware.pMixins_VSPackage
{
    [ComVisible(true)]
    [Guid("B77F7C65-0F9F-422A-A897-C06FDAEC9603")]
    [ProvideObject(typeof(pMixinsVisualStudioCodeGenerateInitializer))]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public class pMixinsVisualStudioCodeGenerateInitializer : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            Log4NetInitializer.Initialize(GetOutputPane((DTE)GetService(typeof(DTE))));

            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            log.Warn("I'm in!");
        }

        protected EnvDTE.OutputWindowPane GetOutputPane(DTE dte)
        {
            if (dte == null)
                return null;

            string windowName = GetType().Name;
            EnvDTE.OutputWindowPane pane = null;
            EnvDTE.Window window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            if (window != null)
            {
                EnvDTE.OutputWindow output = window.Object as EnvDTE.OutputWindow;
                if (output != null)
                {
                    pane = output.ActivePane;
                    if (pane == null || pane.Name != windowName)
                    {
                        for (int ix = output.OutputWindowPanes.Count; ix > 0; ix--)
                        {
                            pane = output.OutputWindowPanes.Item(ix);
                            if (pane.Name == windowName)
                                break;
                        }
                        if (pane == null || pane.Name != windowName)
                            pane = output.OutputWindowPanes.Add(windowName);
                        if (pane != null)
                            pane.Activate();
                    }
                }
            }
            return pane;
        }
    }
}
