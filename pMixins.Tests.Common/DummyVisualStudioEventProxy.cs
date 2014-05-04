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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;

namespace CopaceticSoftware.pMixins.Tests.Common
{
    public class DummyVisualStudioEventProxy : IVisualStudioEventProxy
    {
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
        public event EventHandler<ProjectItemSavedEventArgs> OnProjectItemSaved;
        public event EventHandler<VisualStudioBuildEventArgs> OnBuildBegin;
        public event EventHandler<VisualStudioBuildEventArgs> OnBuildDone;
        public event EventHandler<EventArgs> OnSolutionClosing;
        public event EventHandler<EventArgs> OnSolutionOpening;
    }
}
