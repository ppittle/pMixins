//----------------------------------------------------------------------- 
// <copyright file="MockSolutionCommonTests.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 14, 2014 6:38:06 PM</date> 
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Narrator.Framework.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using Rhino.Mocks;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests
{
    public static class MockSolutionInitializer
    {
        public const string DefaultMixinMethodName = "TestMethod";
        public const int DefaultMixinMethodReturnValue = 42;

        public static MockSolution InitializeWithEmptyProject(this MockSolution s)
        {
            s.Projects.Add(new MockProject());

            return s;
        }

        public static MockSolution InitializeWithNormalClassFile(this MockSolution s)
        {
            s.Projects.Add(
                new MockProject
                {
                    AssemblyReferences = 
                        ReferenceHelper.GetDefaultSystemReferences()
                        .ToList(),
                    MockSourceFiles =
                    {
                        MockSourceFile.CreateDefaultFile()
                    }
                });

            return s;
        }

        public static MockSolution InitializeWithTargetAndMixinInSameClass(this MockSolution s)
        {
            s.Projects.Add(
                new MockProject
                {
                    AssemblyReferences =
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(new[] { ReferenceHelper.GetReferenceToPMixinsDll() })
                        .ToList(),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }

                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target  {}
                                }
                                ",
                        }
                    }
                });

            return s;
        }

        public static MockSolution InitializeWithTargetAndMixinInSeparateClass(this MockSolution s)
        {
            s.Projects.Add(
                new MockProject
                {
                    AssemblyReferences =
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(new[] { ReferenceHelper.GetReferenceToPMixinsDll() })
                        .ToList(),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target  {}
                                }"
                        }
                    }
                });

            return s;
        }

        public static MockSolution InitializeWithTwoTargetsAndMixinInSeparateClass(this MockSolution s)
        {
            s.Projects.Add(
                new MockProject
                {
                    AssemblyReferences =
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(new[] { ReferenceHelper.GetReferenceToPMixinsDll() })
                        .ToList(),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target  {}
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target2.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target2  {}
                                }"
                        }
                    }
                });

            return s;
        }

        public static MockSolution InitializeWithTargetWithTwoMixins(this MockSolution s)
        {
            s.Projects.Add(
                new MockProject
                {
                    AssemblyReferences =
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(new[] { ReferenceHelper.GetReferenceToPMixinsDll() })
                        .ToList(),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin1.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin1{ public int TestMethod1(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin2{ public int TestMethod2(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin1))]                                        
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin2))]  
                                    public partial class Target  {}
                                }"
                        }
                    }
                });

            return s;
        }

        public static MockSolution InitializeWithTargetAndMixinInSeparateProjects(this MockSolution s)
        {
            s.Projects = new List<MockProject>
            {
                new MockProject
                {
                    AssemblyReferences = 
                        ReferenceHelper.GetDefaultSystemReferences()
                        .Union(new []{ReferenceHelper.GetReferenceToPMixinsDll()})
                        .ToList(),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target  {}
                                }"
                        }
                    }
                },
                new MockProject
                {
                    FileName = Path.Combine(MockSolution.MockSolutionFolder, "OtherProject.csproj"),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = Path.Combine(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }                                    
                                }"
                        }
                    }
                },
            };

            s.Projects[1].ProjectReferences.Add(s.Projects[0]);

            return s;
        }
    }

    public static class MockSolutionCommonTestHelper
    {
        #region Events

        public static void FireSolutionOpen(this MockSolutionTestBase mockSolutionTest)
        {
            mockSolutionTest.EventProxy.FireOnSolutionOpening(mockSolutionTest, new EventArgs());
        }

        #endregion

        public static void AssertCompilesAndCanExecuteMethod(
            this MockSourceFile file, 
            MockSolution solution,
            string methodName = MockSolutionInitializer.DefaultMixinMethodName,
            int returnValue = MockSolutionInitializer.DefaultMixinMethodReturnValue)
        {
            MockProject project = solution.Projects.FirstOrDefault(p => p.ContainsFile(file));

            Assert.NotNull(project, "Failed to find Project for File [" + file.FileName + "]");

            AssertCompilesAndCanExecuteMethod<int>(
                project,
                "Testing." + file.Classname,
                methodName,
                returnValue);
        }

        public static void AssertCompilesAndCanExecuteMethod<T>(
            this MockProject project,
            string fullClassName,
            string methodName,
            T expectedReturnValue)
        {
            var compilerResults = project.Compile();

            Assert.False(compilerResults.Errors.HasErrors,
                "Project does not compile: " +
                compilerResults.PrettyPrintErrorList());

            compilerResults
               .ExecuteMethod<T>(
                   fullClassName,
                   methodName)
               .ShouldEqual(expectedReturnValue);
        }

        #region Asserts
        public static void AssertCodeBehindFileWasNotGenerated(this MockSolutionTestBase mockSolutionTest)
        {
            Assert.True(
                mockSolutionTest._MockSolution.AllMockFiles().All(x => !x.FileName.EndsWith("mixin.cs")),
                "Found a mixin.cs code behind file.");

            mockSolutionTest._MockCodeBehindFileHelper.AssertWasNotCalled(
                x => x.GetOrAddCodeBehindFile(Arg<string>.Is.Anything));

            mockSolutionTest._MockFileWrapper.AssertWasNotCalled(
                x => x.WriteAllText(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
        }

        public static void AssertCodeBehindFileWasGenerated(this MockSolutionTestBase mockSolutionTest, string targetFile)
        {
            var codeBehindFile = targetFile.Replace(".cs", ".mixin.cs");

            Assert.True(
                mockSolutionTest._MockSolution.AllMockFiles().Any(
                    x => x.FileName.Equals(codeBehindFile, StringComparison.InvariantCultureIgnoreCase)),
                "Failed to find code behind file [" + codeBehindFile + "]");

            mockSolutionTest._MockCodeBehindFileHelper.AssertWasCalled(
                x => x.GetOrAddCodeBehindFile(Arg<string>.Is.Equal(targetFile)));

            mockSolutionTest._MockFileWrapper.AssertWasCalled(
                x => x.WriteAllText(Arg<string>.Is.Equal(codeBehindFile), Arg<string>.Is.Anything));
        }
        #endregion
    }
}
