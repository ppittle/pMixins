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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
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

        public static MockSolution InitializeWithInvalidTargetFile(this MockSolution s)
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(MixinDoesNotExist))]                                        
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
                            Source = @"
                                namespace Testing{
                                    [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Mixin))]                                        
                                    public partial class Target  {}
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target2.cs"),
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin1.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin1{ public int TestMethod1(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin2.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin2{ public int TestMethod2(){return 42;} }
                                }"
                        },
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
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
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Target.cs"),
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
                    FileName = new FilePath(MockSolution.MockSolutionFolder, "OtherProject.csproj"),
                    MockSourceFiles =
                    {
                        new MockSourceFile
                        {
                            FileName = new FilePath(MockSolution.MockSolutionFolder, "Mixin.cs"),
                            Source = 
                             @"
                                namespace Testing{
                                    public class Mixin{ public int TestMethod(){return 42;} }                                    
                                }"
                        }
                    }
                },
            };

            s.Projects[0].ProjectReferences.Add(s.Projects[1]);

            return s;
        }

        public static MockSolution InitializeWithChainedMixinInSeparateFiles(this MockSolution s)
        {
            s.InitializeWithTargetAndMixinInSeparateClass();

            s.Projects[0].MockSourceFiles.Add(new MockSourceFile("Chained")
            {
                FileName = new FilePath(MockSolution.MockSolutionFolder, "Chained.cs"),
                Source = @"
                    namespace Testing{
                        [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof(Target))]                                        
                        public partial class Chained  {}
                    }"
            });

            return s;
        }
    }

    public static class MockSolutionManipulator
    {
        public static MockSourceFile[] RemoveFiles(this MockSolution solution,
            Func<MockSolution, IEnumerable<MockSourceFile>> sourceFilesFunc)
        {
            var files = sourceFilesFunc(solution);

            return 
                null == files 
                ? new MockSourceFile[0] 
                : files
                    .Select(f => RemoveFile(solution, f))
                    .ToArray();
        }

        public static MockSourceFile RemoveFile(this MockSolution solution,
            Func<MockSolution, MockSourceFile> sourceFileFunc)
        {
            return RemoveFile(solution, sourceFileFunc(solution));
        }

        public static MockSourceFile RemoveFile(this MockSolution solution,
            MockSourceFile sourceFile)
        {
            if (null == sourceFile)
                return null;

            //Load Project
            var project = solution.Projects.FirstOrDefault(p => p.ContainsFile(sourceFile));

            Assert.NotNull(project, "Could not find Project that contains [{0}]", sourceFile.FileName);

            //Remove File from Project
            project.MockSourceFiles =
                project.MockSourceFiles
                    .Where(f => !f.FileName.Equals(sourceFile.FileName))
                    .ToList();

            return sourceFile;
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

        public static void AddMockSourceFile(
            this MockSolutionTestBase mockSolutionTest,
            Func<MockSolution, MockProject> projectSelector,
            string initialSource,
            string fileName = MockSourceFile.DefaultMockFileName)
        {
            AddMockSourceFile(
                mockSolutionTest,
                projectSelector,
                new MockSourceFile(fileName)
                {
                    Source = initialSource
                });
        }

        public static void AddMockSourceFile(
            this MockSolutionTestBase mockSolutionTest,
            Func<MockSolution, MockProject> projectSelector,
            MockSourceFile mockSourceFile)
        {
            var project = projectSelector(mockSolutionTest._MockSolution);

            project.MockSourceFiles.Add(mockSourceFile);

            //FireOnItemCreated
            mockSolutionTest.EventProxy.FireOnProjectItemAdded(mockSolutionTest,
                new ProjectItemAddedEventArgs
                {
                    ClassFullPath = mockSourceFile.FileName,
                    ProjectFullPath = project.FileName
                });
        }

        public static void UpdateMockSourceFileSource
            (this MockSolutionTestBase mockSolutionTest,
                Func<MockSolution, MockSourceFile> mockSourceFileSelector,
                Func<MockSourceFile, string> updatedSourceFunc )
        {
            var sourceFile = mockSourceFileSelector(mockSolutionTest._MockSolution);

            Assert.NotNull(sourceFile, "mockSourceFileSelector returned null");

            UpdateMockSourceFileSource(
                mockSolutionTest,
                mockSourceFileSelector(mockSolutionTest._MockSolution),
                updatedSourceFunc(sourceFile));
        }

        public static void UpdateMockSourceFileSource
            (this MockSolutionTestBase mockSolutionTest,
                MockSourceFile mockSourceFile,
                string updatedSource)
        {
            mockSourceFile.Source = updatedSource;

            var project = mockSolutionTest._MockSolution.Projects.FirstOrDefault(
                p => p.ContainsFile(mockSourceFile));

            Assert.IsNotNull(project, 
                "Could not find Project that has Source File [{0}", mockSourceFile.FileName );

            mockSolutionTest.EventProxy.FireOnProjectItemSaved(mockSolutionTest,
                new ProjectItemSavedEventArgs
                {
                    ClassFullPath = mockSourceFile.FileName,
                    ProjectFullPath = project.FileName
                });
        }

        #region Asserts
        public static CompilerResults AssertCompiles(this MockProject project)
        {
            var compilerResults = project.Compile();

            Assert.False(compilerResults.Errors.HasErrors,
                "Project does not compile: " +
                compilerResults.PrettyPrintErrorList());

            return compilerResults;
        }

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
            var compilerResults = AssertCompiles(project);

            compilerResults
               .ExecuteMethod<T>(
                   fullClassName,
                   methodName)
               .ShouldEqual(expectedReturnValue);
        }
        
        public static void AssertCodeBehindFileWasNotGenerated(this MockSolutionTestBase mockSolutionTest)
        {
            Assert.True(
                mockSolutionTest._MockSolution.AllMockFiles().All(x => !x.FileName.Extension.Equals("mixin.cs")),
                "Found a mixin.cs code behind file.");

            mockSolutionTest._MockCodeBehindFileHelper.AssertWasNotCalled(
                x => x.GetOrAddCodeBehindFile(Arg<FilePath>.Is.Anything));

            mockSolutionTest._MockFileWrapper.AssertWasNotCalled(
                x => x.WriteAllText(Arg<FilePath>.Is.Anything, Arg<string>.Is.Anything));
        }

        public static void AssertCodeBehindFileWasGenerated(this MockSolutionTestBase mockSolutionTest,
            MockSourceFile targetFile)
        {
            mockSolutionTest.AssertCodeBehindFileWasGenerated(targetFile.FileName);
        }

        public static void AssertCodeBehindFileWasGenerated(this MockSolutionTestBase mockSolutionTest, FilePath targetFile)
        {
            var codeBehindFile = new FilePath(targetFile.FullPath.ToLower().Replace(".cs", ".mixin.cs"));

            Assert.True(
                mockSolutionTest._MockSolution.AllMockFiles().Any(
                    x => x.FileName.Equals(codeBehindFile)),
                "Failed to find code behind file [" + codeBehindFile + "]");

            mockSolutionTest._MockCodeBehindFileHelper.AssertWasCalled(
                x => x.GetOrAddCodeBehindFile(Arg<FilePath>.Is.Equal(targetFile)));

            mockSolutionTest._MockFileWrapper.AssertWasCalled(
                x => x.WriteAllText(Arg<FilePath>.Is.Equal(codeBehindFile), Arg<string>.Is.Anything));
        }
        #endregion
    }
}
