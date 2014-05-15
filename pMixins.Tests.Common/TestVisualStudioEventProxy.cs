//----------------------------------------------------------------------- 
// <copyright file="DummyVisualStudioEventProxy.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, March 17, 2014 10:56:28 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using log4net;

namespace CopaceticSoftware.pMixins.Tests.Common
{
    public class TestVisualStudioEventProxy : IVisualStudioEventProxy
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Dispose()
        {
            
        }

        public event EventHandler<ProjectAddedEventArgs> OnProjectAdded;
        public event EventHandler<ProjectRemovedEventArgs> OnProjectRemoved;
        public event EventHandler<ProjectReferenceAddedEventArgs> OnProjectReferenceAdded;
        public event EventHandler<ProjectReferenceRemovedEventArgs> OnProjectReferenceRemoved;
        public event EventHandler<ProjectItemAddedEventArgs> OnProjectItemAdded;
        public event EventHandler<ProjectItemRemovedEventArgs> OnProjectItemRemoved;
        public event EventHandler<ProjectItemRenamedEventArgs> OnProjectItemRenamed;
        public event EventHandler<ProjectItemOpenedEventArgs> OnProjectItemOpened;
        public event EventHandler<ProjectItemClosedEventArgs> OnProjectItemClosed;
        public event EventHandler<ProjectItemSavedEventArgs> OnProjectItemSaved;
        public event EventHandler<ProjectItemSavedEventArgs> OnProjectItemSaveComplete;
        public event EventHandler<VisualStudioBuildEventArgs> OnBuildBegin;
        public event EventHandler<VisualStudioBuildEventArgs> OnBuildDone;
        public event EventHandler<EventArgs> OnSolutionClosing;
        public event EventHandler<EventArgs> OnSolutionOpening;
        public event EventHandler<CodeGeneratedEventArgs> OnCodeGenerated;

        public void FireOnProjectAdded(object sender, ProjectAddedEventArgs eventArgs)
        {
            _log.Info("OnProjectAdded");

            if (null != OnProjectAdded)
                OnProjectAdded(sender, eventArgs);
        }

        public void FireOnProjectRemoved(object sender, ProjectRemovedEventArgs eventArgs)
        {
            _log.Info("OnProjectRemoved");

            if (null != OnProjectRemoved)
                OnProjectRemoved(sender, eventArgs);
        }

        public void FireOnProjectReferenceAdded(object sender, ProjectReferenceAddedEventArgs eventArgs)
        {
            _log.Info("OnProjectReferenceAdded");

            if (null != OnProjectReferenceAdded)
                OnProjectReferenceAdded(sender, eventArgs);
        }

        public void FireOnProjectReferenceRemoved(object sender, ProjectReferenceRemovedEventArgs eventArgs)
        {
            _log.Info("OnProjectReferenceRemoved");

            if (null != OnProjectReferenceRemoved)
                OnProjectReferenceRemoved(sender, eventArgs);
        }

        public void FireOnProjectItemAdded(object sender, ProjectItemAddedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemAdded");

            if (null != OnProjectItemAdded)
                OnProjectItemAdded(sender, eventArgs);
        }

        public void FireOnProjectItemRemoved(object sender, ProjectItemRemovedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemRemoved");

            if (null != OnProjectItemRemoved)
                OnProjectItemRemoved(sender, eventArgs);
        }

        public void FireOnProjectItemRenamed(object sender, ProjectItemRenamedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemRenamed");

            if (null != OnProjectItemRenamed)
                OnProjectItemRenamed(sender, eventArgs);
        }

        public void FireOnProjectItemOpened(object sender, ProjectItemOpenedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemOpened");

            if (null != OnProjectItemOpened)
                OnProjectItemOpened(sender, eventArgs);
        }

        public void FireOnProjectItemClosed(object sender, ProjectItemClosedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemClosed");

            if (null != OnProjectItemClosed)
                OnProjectItemClosed(sender, eventArgs);
        }

        public void FireOnProjectItemSaved(object sender, ProjectItemSavedEventArgs eventArgs)
        {
            _log.Info("OnProjectItemSaved");

            if (null != OnProjectItemSaved)
                OnProjectItemSaved(sender, eventArgs);

            _log.Info("OnProjectItemSaveComplete");

            if (null != OnProjectItemSaveComplete)
                OnProjectItemSaveComplete(sender, eventArgs);
        }

        public void FireOnBuildBegin(object sender, VisualStudioBuildEventArgs eventArgs)
        {
            _log.Info("OnBuildBegin");

            if (null != OnBuildBegin)
                OnBuildBegin(sender, eventArgs);
        }

        public void FireOnBuildDone(object sender, VisualStudioBuildEventArgs eventArgs)
        {
            _log.Info("OnBuildDone");

            if (null != OnBuildDone)
                OnBuildDone(sender, eventArgs);
        }

        public void FireOnSolutionClosing(object sender, EventArgs eventArgs)
        {
            _log.Info("OnSolutionClosing");

            if (null != OnSolutionClosing)
                OnSolutionClosing(sender, eventArgs);
        }

        public void FireOnSolutionOpening(object sender, EventArgs eventArgs)
        {
            _log.Info("OnSolutionOpening");

            if (null != OnSolutionOpening)
                OnSolutionOpening(sender, eventArgs);
        }
        
        public void FireOnCodeGenerated(object sender, CodeGeneratorResponse response)
        {
            _log.Info("OnCodeGenerated");

           if (null != OnCodeGenerated)
               OnCodeGenerated(sender, new CodeGeneratedEventArgs { Response = response });
        }
    }
}
