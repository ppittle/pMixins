//----------------------------------------------------------------------- 
// <copyright file="MixinCodeBehindFileIsRecreatedOnItmeSave.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, May 12, 2014 11:38:08 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator
{
    public class MixinCodeBehindFileIsRecreatedOnItmeSave : OnItemSaveForPMixinFileWithNoDependencies
    {
        public override void MainSetup()
        {
            base.MainSetup();

            //Simulate a Delete on mixin code behind.
            var codeBehind = _MockSolution.AllMockFiles().FirstOrDefault(x => x.FileName.EndsWith(".mixin.cs"));

            Assert.True(null != codeBehind, "Code Behind File was not generated!");

            EventProxy.FireOnProjectItemRemoved(this, 
                new ProjectItemRemovedEventArgs
                {
                    ClassFullPath = codeBehind.FileName,
                    ProjectFullPath = _MockSolution.Projects[0].FileName
                });

            //Simulate Item Saved
            EventProxy.FireOnProjectItemSaved(this,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = _sourceFile.FileName,
                    ProjectFullPath = _MockSolution.Projects[0].FileName
                });
        }

        //[Test] - Base Class test is still valid and does not need to be repeated here.
    }
}
