//----------------------------------------------------------------------- 
// <copyright file="BaseCodeGeneratorWithSite.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 29, 2013 6:53:26 PM</date> 
// Licensed under the Apache License, Version 2.0,
// you may not use this file except in compliance with one of the Licenses.
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


/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;

namespace Microsoft.Samples.VisualStudio.GeneratorSample
{
    ///http://code.msdn.microsoft.com/windowsdesktop/Single-File-Generator-94d856d4
    /// <summary>
    /// Base code generator with site implementation
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseCodeGeneratorWithSite : BaseCodeGenerator, IObjectWithSite
    {
        private object site = null;
        private CodeDomProvider codeDomProvider = null;
        private ServiceProvider serviceProvider = null;

        #region IObjectWithSite Members

        /// <summary>
        /// GetSite method of IOleObjectWithSite
        /// </summary>
        /// <param name="riid">interface to get</param>
        /// <param name="ppvSite">IntPtr in which to stuff return value</param>
        void Microsoft.VisualStudio.OLE.Interop.IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppvSite)
        {
            if (site == null)
            {
                throw new COMException("object is not sited", VSConstants.E_FAIL);
            }

            IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
            IntPtr intPointer = IntPtr.Zero;
            Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

            if (intPointer == IntPtr.Zero)
            {
                throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
            }

            ppvSite = intPointer;
        }

        /// <summary>
        /// SetSite method of IOleObjectWithSite
        /// </summary>
        /// <param name="pUnkSite">site for this object to use</param>
        void Microsoft.VisualStudio.OLE.Interop.IObjectWithSite.SetSite(object pUnkSite)
        {
            site = pUnkSite;
            codeDomProvider = null;
            serviceProvider = null;
        }

        #endregion

        /// <summary>
        /// Demand-creates a ServiceProvider
        /// </summary>
        private ServiceProvider SiteServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    serviceProvider = new ServiceProvider(site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
                    Debug.Assert(serviceProvider != null, "Unable to get ServiceProvider from site object.");
                }
                return serviceProvider;
            }
        }

        /// <summary>
        /// Method to get a service by its GUID
        /// </summary>
        /// <param name="serviceGuid">GUID of service to retrieve</param>
        /// <returns>An object that implements the requested service</returns>
        protected object GetService(Guid serviceGuid)
        {
            return SiteServiceProvider.GetService(serviceGuid);
        }

        /// <summary>
        /// Method to get a service by its Type
        /// </summary>
        /// <param name="serviceType">Type of service to retrieve</param>
        /// <returns>An object that implements the requested service</returns>
        protected object GetService(Type serviceType)
        {
            return SiteServiceProvider.GetService(serviceType);
        }

        /// <summary>
        /// Returns a CodeDomProvider object for the language of the project containing
        /// the project item the generator was called on
        /// </summary>
        /// <returns>A CodeDomProvider object</returns>
        protected virtual CodeDomProvider GetCodeProvider()
        {
            if (codeDomProvider == null)
            {
                //Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
                IVSMDCodeDomProvider provider = GetService(typeof(SVSMDCodeDomProvider)) as IVSMDCodeDomProvider;
                if (provider != null)
                {
                    codeDomProvider = provider.CodeDomProvider as CodeDomProvider;
                }
                else
                {
                    //In the case where no language specific CodeDom is available, fall back to C#
                    codeDomProvider = CodeDomProvider.CreateProvider("C#");
                }
            }
            return codeDomProvider;
        }

        /// <summary>
        /// Gets the default extension of the output file from the CodeDomProvider
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            CodeDomProvider codeDom = GetCodeProvider();
            Debug.Assert(codeDom != null, "CodeDomProvider is NULL.");
            string extension = codeDom.FileExtension;
            if (extension != null && extension.Length > 0)
            {
                extension = "." + extension.TrimStart(".".ToCharArray());
            }
            return extension;
        }

        /// <summary>
        /// Returns the EnvDTE.ProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
        protected ProjectItem GetProjectItem()
        {
            object p = GetService(typeof(ProjectItem));
            Debug.Assert(p != null, "Unable to get Project Item.");
            return (ProjectItem)p;
        }

        /// <summary>
        /// Returns the EnvDTE.Project object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The EnvDTE.Project object of the project containing the project item the code generator was called on
        /// </returns>
        protected Project GetProject()
        {
            return GetProjectItem().ContainingProject;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProjectItem object that corresponds to the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>The VSLangProj.VSProjectItem of the project item the code generator was called on</returns>
        protected VSProjectItem GetVSProjectItem()
        {
            return (VSProjectItem)GetProjectItem().Object;
        }

        /// <summary>
        /// Returns the VSLangProj.VSProject object of the project containing the project item the code 
        /// generator was called on
        /// </summary>
        /// <returns>
        /// The VSLangProj.VSProject object of the project containing the project item 
        /// the code generator was called on
        /// </returns>
        protected VSProject GetVSProject()
        {
            return (VSProject)GetProject().Object;
        }

        protected EnvDTE.OutputWindowPane GetOutputPane(DTE dte)
        {
            try
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
            catch
            {
                return null;
            }
        }
    }
}
