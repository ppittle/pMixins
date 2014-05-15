//----------------------------------------------------------------------- 
// <copyright file="OnItemSaveCodeGeneratorTestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 10, 2014 8:00:47 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnSolutionOpenCodeGenerator;
using CopaceticSoftware.pMixins.VisualStudio.CodeGenerators;
using Ninject;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator
{
    public abstract class OnItemSaveCodeGeneratorTestBase : OnSolutionItemOpenCodeGeneratorTestBase
    {
// ReSharper disable NotAccessedField.Global
        //Instantiate the OnItemSaveCodeGenerator so it will subscribe to events
        protected pMixinsOnItemSaveCodeGenerator _PMixinsOnItemSaveCodeGenerator;
// ReSharper restore NotAccessedField.Global

        public override void MainSetup()
        {
            base.MainSetup();

            _PMixinsOnItemSaveCodeGenerator = TestSpecificKernel.Get<pMixinsOnItemSaveCodeGenerator>();
        }
    }
}
