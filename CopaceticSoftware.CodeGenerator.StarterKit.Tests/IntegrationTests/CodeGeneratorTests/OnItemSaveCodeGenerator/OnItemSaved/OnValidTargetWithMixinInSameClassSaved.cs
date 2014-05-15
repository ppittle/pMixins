//----------------------------------------------------------------------- 
// <copyright file="OnValidTargetWithMixinInSameClassSaved.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, May 15, 2014 3:49:24 PM</date> 
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

using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnProjectItemAdded;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnItemSaved
{
    [TestFixture]
    public class OnValidTargetWithMixinInSameClassSaved : OnValidTargetWithMixinInSameClassAddedToEmptyProject
    {
        public override void MainSetup()
        {
            base.MainSetup();

            //Update Normal Class (add a space to the source)
            this.UpdateMockSourceFileSource(
                s => s.AllMockSourceFiles.First(),
                f => f.Source += " ");
        }

        //[Test] - Base class tests are still valid in this context
        //public void CanExecuteMixedInMethod()
    }
}
