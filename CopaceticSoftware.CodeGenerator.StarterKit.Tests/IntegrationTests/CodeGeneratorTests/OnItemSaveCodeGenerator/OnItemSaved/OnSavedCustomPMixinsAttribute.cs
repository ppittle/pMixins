//----------------------------------------------------------------------- 
// <copyright file="OnSavedCustomPMixinsAttribute.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, September 2, 2014 12:07:34 AM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using NUnit.Framework;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.CodeGeneratorTests.OnItemSaveCodeGenerator.OnItemSaved
{
    [TestFixture]
    public class OnSavedWithCustomPMixinsAttribute : OnItemSaveCodeGeneratorTestBase
    {
        protected override void MainSetupInitializeSolution()
        {
            _MockSolution.InitializeWithEmptyProject();

            _MockSolution.Projects[0].MockSourceFiles = new[]
            {
                new MockSourceFile("Mixin")
                {
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin.cs"),
                    Source = @"
                        namespace Testing{
                            public class Mixin{ 
                                public int TestMethod(){return 42;}                                 
                            }
                        }"
                },
                new MockSourceFile("CustomAttribute")
                {
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "CustomAttribute.cs"),
                    Source = @"
                        namespace Testing{
                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                            public class CustomAttribute : System.Attribute  {}
                        }"
                },
                new MockSourceFile("Target")
                {
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
                    Source = @"
                        namespace Testing{
                            [Custom]                                        
                            public partial class Target  {}
                        }"
                },
                new MockSourceFile("Mixin2")
                {
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                    Source = @"
                        namespace Testing{                            
                            public class Mixin2 
                            {
                                public int Mixin2Method(){ return -1;}
                            }
                        }"
                }
            };

            //Fix Project References
            _MockSolution.Projects[0].AssemblyReferences =
                ReferenceHelper.GetDefaultSystemReferences()
                    .Union(new[] { ReferenceHelper.GetReferenceToPMixinsDll() })
                    .ToList();
        }

        public override void MainSetup()
        {
            base.MainSetup();

            //Add a new method to the Mixin class
            this.UpdateMockSourceFileSource(
                _MockSolution.Projects[0].MockSourceFiles[0],
                @"
                    namespace Testing{
                        public class Mixin{ 
                            public int TestMethod(){return 42;} 
                            public int NewMethod(){ return 24;}
                        }
                    }");
        }

        [Test]
        public void CanExecuteChainedMixedInMethods()
        {
            _MockSolution.Projects[0]
                .AssertCompilesAndCanExecuteMethod(
                    "Testing.Target",
                    "TestMethod",
                    42);

            _MockSolution.Projects[0]
                .AssertCompilesAndCanExecuteMethod(
                    "Testing.Target",
                    "NewMethod",
                    24);
        }

        [Test]
        public void OnSaveCustomAttributeFileWithNewMixin()
        {
            this.UpdateMockSourceFileSource(
               _MockSolution.Projects[0].MockSourceFiles[1],
               @"
                namespace Testing{
                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin2))]                                        
                    public class CustomAttribute : System.Attribute  {}
                }");

            _MockSolution.Projects[0]
                .AssertCompilesAndCanExecuteMethod(
                    "Testing.Target",
                    "Mixin2Method",
                    -1);
        }
    }
}
