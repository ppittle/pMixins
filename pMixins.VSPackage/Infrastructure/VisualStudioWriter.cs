//----------------------------------------------------------------------- 
// <copyright file="VisualStudioWriter.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 4:07:45 PM</date> 
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
using System.Net;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CopaceticSoftware.pMixins_VSPackage.Infrastructure
{
    public class VisualStudioWriter : IVisualStudioWriter
    {
        private EnvDTE.OutputWindowPane _outputWindowPane;
        private ErrorListProvider _errorListProvider;

        //http://geekswithblogs.net/onlyutkarsh/archive/2013/08/11/using-visual-studio-status-bar-in-your-extensions.aspx
        private IVsStatusbar _statusbar;

        public VisualStudioWriter(DTE dte, System.IServiceProvider serviceProvider)
        {
            if (dte == null)
            { 
                Debug.Write("Visual Studio Writer was passed a null DTE");

                if(System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                
                return;
            }
            
            _outputWindowPane = LoadOutputWindowPane(dte);
            _errorListProvider = LoadErrorListPane(serviceProvider);
            _statusbar = serviceProvider.GetService(typeof (SVsStatusbar)) as IVsStatusbar;
        }

        private EnvDTE.OutputWindowPane LoadOutputWindowPane(DTE dte)
        {
            const string windowName = "pMixins Code Generator";
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

        /// <summary>
        /// http://mhusseini.wordpress.com/2013/05/30/write-to-visual-studios-error-list/
        /// </summary>
        [DebuggerStepThrough]
        private ErrorListProvider LoadErrorListPane(System.IServiceProvider serviceProvider)
        {
            return new ErrorListProvider(serviceProvider);
        }

        [DebuggerStepThrough]
        public void GeneratorError(string message, uint line, uint column)
        {
            AddTask(TaskErrorCategory.Error, message, (int)line, (int)column);
        }

        [DebuggerStepThrough]
        public void GeneratorWarning(string message, uint line, uint column)
        {
            AddTask(TaskErrorCategory.Warning, message, (int)line, (int)column);
        }

        [DebuggerStepThrough]
        public void GeneratorMessage(string message, uint line, uint column)
        {
            AddTask(TaskErrorCategory.Message, message, (int)line, (int)column);
        }

        [DebuggerStepThrough]
        public void ClearErrorList()
        {
            _errorListProvider.Tasks.Clear();
        }

        [DebuggerStepThrough]
        private void AddTask(TaskErrorCategory category, string message, int line, int column)
        {
            _errorListProvider.Tasks.Add(
                new ErrorTask
                {
                    Category = TaskCategory.User,
                    ErrorCategory = category,
                    Line = line,
                    Column = column,
                    Text = message
                });
        }

        [DebuggerStepThrough]
        public void OutputString(string s)
        {
            _outputWindowPane.OutputString(s);
        }

        [DebuggerStepThrough]
        public void WriteToStatusBar(string s)
        {
            if (null == _statusbar)
                return;

            int frozen;

            _statusbar.IsFrozen(out frozen);

            if (0 == frozen)
                _statusbar.SetText(s);
        }

        [DebuggerStepThrough]
        public void SetStatusProgress(uint cookie, int progress, string label, uint complete, uint total)
        {
            if (null == _statusbar)
                return;

            _statusbar.Progress(cookie, progress, label, complete, total);
        }

        [DebuggerStepThrough]
        public void ClearStatusBar()
        {
            if (null == _statusbar)
                return;

            _statusbar.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (null != _errorListProvider)
                    _errorListProvider.Dispose();
            }

            _outputWindowPane = null;
            _errorListProvider = null;
            _statusbar = null;

            _disposed = true;
        }
    }
}
