//----------------------------------------------------------------------- 
// <copyright file="MockSolutionFileContents.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, May 7, 2014 2:07:06 PM</date> 
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

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    public interface IMockFile
    {
        string FileName { get; }
        string RenderFile();
    }

    public class MockSolution : IMockFile
    {
        public const string MockSolutionFileName = @"c:\test\MockSolution.sln";

        public MockSolution(string filename = MockSolutionFileName)
        {
            FileName = filename;
            Projects = new List<MockProject>();
        }

        public string FileName { get; set; }

        public IList<MockProject> Projects { get; set; }

        public IEnumerable<MockSourceFile> AllMockSourceFiles { get { return Projects.SelectMany(p => p.MockSourceFiles); } }

        public IEnumerable<IMockFile> AllMockFiles()
        {
            return
                AllMockSourceFiles.Cast<IMockFile>()
                    .Union(Projects)
                    .Union(new[] {this});
        }

        public string RenderFile()
        {
            return string.Format(
                @"  Microsoft Visual Studio Solution File, Format Version 12.00
                    # Visual Studio 2013
                    VisualStudioVersion = 12.0.30110.0
                    MinimumVisualStudioVersion = 10.0.40219.1
                    {0}
                    Global
	                    GlobalSection(SolutionConfigurationPlatforms) = preSolution
		                    Debug|Any CPU = Debug|Any CPU
		                    Release|Any CPU = Release|Any CPU
	                    EndGlobalSection
	                    GlobalSection(ProjectConfigurationPlatforms) = postSolution
		                    {1}
	                    EndGlobalSection
	                    GlobalSection(SolutionProperties) = preSolution
		                    HideSolutionNode = FALSE
	                    EndGlobalSection
                    EndGlobal",

                    string.Join(Environment.NewLine,
                        Projects.Select(
                            p =>
                                string.Format(
                                    @"Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{0}"", ""{1}"", ""{2}""
                                        EndProject",
                                    p.Title,
                                    p.FileName,
                                    p.Guid))), 

                    string.Join(Environment.NewLine,
                    Projects.Select(
                        p =>
                            string.Format(
                                @"{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		                          {0}.Debug|Any CPU.Build.0 = Debug|Any CPU
		                          {0}.Release|Any CPU.ActiveCfg = Release|Any CPU
		                          {0}.Release|Any CPU.Build.0 = Release|Any CPU",
                                p.Guid)))
                );
        }
    }

    public class MockProject : IMockFile
    {
        public MockProject(string filename = @"c:\test\MockSolution\MockProject.csproj")
        {
            FileName = filename;
            Title = Path.GetFileNameWithoutExtension(filename);

            AssemblyReferences = new List<string>();
            ProjectReferences = new List<MockProject>();
            MockSourceFiles = new List<MockSourceFile>();
        }

        public readonly Guid Guid = System.Guid.NewGuid();
        public string FileName { get; set; }
        public string Title { get; set; }
        public IList<string> AssemblyReferences { get; set; }
        public IList<MockProject> ProjectReferences { get; set; }
        public IList<MockSourceFile> MockSourceFiles { get; set; }

        public string RenderFile()
        {
            return string.Format(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                      <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                      <PropertyGroup>
                        <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                        <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                        <ProjectGuid>{0}</ProjectGuid>
                        <OutputType>Library</OutputType>
                        <AppDesignerFolder>Properties</AppDesignerFolder>
                        <RootNamespace>{1}</RootNamespace>
                        <AssemblyName>{1}</AssemblyName>
                        <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                        <FileAlignment>512</FileAlignment>
                        <DesignTimeBuild>true</DesignTimeBuild>
                      </PropertyGroup>
                      <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                        <PlatformTarget>AnyCPU</PlatformTarget>
                        <DebugSymbols>true</DebugSymbols>
                        <DebugType>full</DebugType>
                        <Optimize>false</Optimize>
                        <OutputPath>bin\Debug\</OutputPath>
                        <DefineConstants>DEBUG;TRACE</DefineConstants>
                        <ErrorReport>prompt</ErrorReport>
                        <WarningLevel>4</WarningLevel>
                      </PropertyGroup>
                      <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
                        <PlatformTarget>AnyCPU</PlatformTarget>
                        <DebugType>pdbonly</DebugType>
                        <Optimize>true</Optimize>
                        <OutputPath>bin\Release\</OutputPath>
                        <DefineConstants>TRACE</DefineConstants>
                        <ErrorReport>prompt</ErrorReport>
                        <WarningLevel>4</WarningLevel>
                      </PropertyGroup>
                      <PropertyGroup>
                        <StartupObject />
                      </PropertyGroup>
                      <ItemGroup>
                        {2}
                        <Reference Include=""System"" />
                      </ItemGroup>
                      <ItemGroup>
                        {3}
                      </ItemGroup>
                      <ItemGroup>
                        {4}
                      </ItemGroup>
                      <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
                      <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
                           Other similar extension points exist, see Microsoft.Common.targets.
                      <Target Name=""BeforeBuild"">
                      </Target>
                      <Target Name=""AfterBuild"">
                      </Target>
                      -->
                    </Project>",
                Guid,
                Title,

                //AssemblyReferences
                string.Join(
                    Environment.NewLine,
                    AssemblyReferences.Select(
                        r =>
                            string.Format(
                                @"<Reference Include=""{0}"">
                                    <HintPath>{1}</HintPath>
                                  </Reference>",
                                Path.GetFileNameWithoutExtension(r),
                                r)
                        )),
                
                //Files
                string.Join(
                    Environment.NewLine,
                    MockSourceFiles.Select(
                        f =>
                            string.Format(
                                @"<Compile Include=""{0}"" />",
                                f.FileName)
                        )),

                //ProjectReferences
                string.Join(
                    Environment.NewLine,
                    ProjectReferences.Select(
                        p =>
                            string.Format(
                                @"<ProjectReference Include=""{0}"">
                                      <Project>{1}</Project>
                                      <Name>{2}</Name>
                                    </ProjectReference>",
                                p.FileName,
                                p.Guid,
                                Path.GetFileNameWithoutExtension(p.FileName))
                        )));
        }
    }

    public class MockSourceFile : IMockFile
    {
        public MockSourceFile() { Source = string.Empty;}

        public string FileName { get; set; }
        public string Source { get; set; }

        public string RenderFile() { return Source;}

        public static MockSourceFile CreateDefaultFile(
            string filename = @"c:\test\MockSolution\MockFile.cs")
        {
            return new MockSourceFile
            {
                FileName = filename,
                Source =
                    string.Format(
                        @"
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;

                        namespace CreateDefaultFile
                        {{
                            public class {0}
                            {{
                            }}
                        }}",
                        Path.GetFileNameWithoutExtension(filename))
            };
        }
    }

    public static class ReferenceHelper
    {
        public static string GetReferenceToDllInTestProject(string dllName)
        {
            return
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        dllName));
        }

        public static string GetReferenceToPMixinsDll()
        {
            return ReferenceHelper.GetReferenceToDllInTestProject("CopaceticSoftware.pMixins.dll");
        }
    }

public static class MockSolutionFileContents
    {
       

        public static class Solution
        {
            public const string SolutionFileName = @"c:\test\MockSolution.sln";

            #region public const string SolutionFileWithNoProject
            public const string SolutionFileWithNoProject =
                @"
                    Microsoft Visual Studio Solution File, Format Version 12.00
                    # Visual Studio 2013
                    VisualStudioVersion = 12.0.30110.0
                    MinimumVisualStudioVersion = 10.0.40219.1
                    Global
	                    GlobalSection(SolutionProperties) = preSolution
		                    HideSolutionNode = FALSE
	                    EndGlobalSection
                    EndGlobal";
            #endregion

            #region public const string SolutionFileWithMainProject
            public const string SolutionFileWithMainProject = 
                @"  Microsoft Visual Studio Solution File, Format Version 12.00
                    # Visual Studio 2013
                    VisualStudioVersion = 12.0.30110.0
                    MinimumVisualStudioVersion = 10.0.40219.1
                    Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""MainProject"", ""MainProject\MainProject.csproj"", ""{A95548D8-E945-49B9-B000-47F417F2114F}""
                    EndProject
                    Global
	                    GlobalSection(SolutionConfigurationPlatforms) = preSolution
		                    Debug|Any CPU = Debug|Any CPU
		                    Release|Any CPU = Release|Any CPU
	                    EndGlobalSection
	                    GlobalSection(ProjectConfigurationPlatforms) = postSolution
		                    {A95548D8-E945-49B9-B000-47F417F2114F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		                    {A95548D8-E945-49B9-B000-47F417F2114F}.Debug|Any CPU.Build.0 = Debug|Any CPU
		                    {A95548D8-E945-49B9-B000-47F417F2114F}.Release|Any CPU.ActiveCfg = Release|Any CPU
		                    {A95548D8-E945-49B9-B000-47F417F2114F}.Release|Any CPU.Build.0 = Release|Any CPU
	                    EndGlobalSection
	                    GlobalSection(SolutionProperties) = preSolution
		                    HideSolutionNode = FALSE
	                    EndGlobalSection
                    EndGlobal
                    ";
            #endregion
        }

        public static class MainProject
        {
            public const string ProjectFileName = @"c:\test\MainProject\MainProject.csproj";

            #region public static readonly string ProjectFileWithNoClasses
            public static readonly string ProjectFileWithNoClasses = 
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                  <PropertyGroup>
                    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                    <ProjectGuid>{A95548D8-E945-49B9-B000-47F417F2114F}</ProjectGuid>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>MainProject</RootNamespace>
                    <AssemblyName>MainProject</AssemblyName>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                    <FileAlignment>512</FileAlignment>
                    <DesignTimeBuild>true</DesignTimeBuild>
                  </PropertyGroup>
                  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                    <PlatformTarget>AnyCPU</PlatformTarget>
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\Debug\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
                    <PlatformTarget>AnyCPU</PlatformTarget>
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\Release\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup>
                    <StartupObject />
                  </PropertyGroup>
                    <ItemGroup>
                    <Reference Include=""CopaceticSoftware.pMixins"">
                      <HintPath>" + ReferenceHelper.GetReferenceToDllInTestProject("CopaceticSoftware.pMixins.dll") + @"</HintPath>
                    </Reference>
                  </ItemGroup>
                  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
                  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
                       Other similar extension points exist, see Microsoft.Common.targets.
                  <Target Name=""BeforeBuild"">
                  </Target>
                  <Target Name=""AfterBuild"">
                  </Target>
                  -->
                </Project>
                ";
            #endregion

            #region public static readonly string ProjectFileWithBasicClass
            public static readonly string ProjectFileWithBasicClass = 
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
                  <PropertyGroup>
                    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
                    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
                    <ProjectGuid>{A95548D8-E945-49B9-B000-47F417F2114F}</ProjectGuid>
                    <OutputType>Library</OutputType>
                    <AppDesignerFolder>Properties</AppDesignerFolder>
                    <RootNamespace>MainProject</RootNamespace>
                    <AssemblyName>MainProject</AssemblyName>
                    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
                    <FileAlignment>512</FileAlignment>
                    <DesignTimeBuild>true</DesignTimeBuild>
                  </PropertyGroup>
                  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
                    <PlatformTarget>AnyCPU</PlatformTarget>
                    <DebugSymbols>true</DebugSymbols>
                    <DebugType>full</DebugType>
                    <Optimize>false</Optimize>
                    <OutputPath>bin\Debug\</OutputPath>
                    <DefineConstants>DEBUG;TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
                    <PlatformTarget>AnyCPU</PlatformTarget>
                    <DebugType>pdbonly</DebugType>
                    <Optimize>true</Optimize>
                    <OutputPath>bin\Release\</OutputPath>
                    <DefineConstants>TRACE</DefineConstants>
                    <ErrorReport>prompt</ErrorReport>
                    <WarningLevel>4</WarningLevel>
                  </PropertyGroup>
                  <PropertyGroup>
                    <StartupObject />
                  </PropertyGroup>
                    <ItemGroup>
                     <Reference Include=""CopaceticSoftware.pMixins"">
                      <HintPath>" + ReferenceHelper.GetReferenceToDllInTestProject("CopaceticSoftware.pMixins.dll") + @"</HintPath>
                    </Reference>
                  </ItemGroup>
                <ItemGroup>
                    <Compile Include=""BasicClass.cs"" />
                  </ItemGroup>
                  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
                  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
                       Other similar extension points exist, see Microsoft.Common.targets.
                  <Target Name=""BeforeBuild"">
                  </Target>
                  <Target Name=""AfterBuild"">
                  </Target>
                  -->
                </Project>";
            #endregion

            public static class ProjectItems
            {
                public static class BasicClass
                {
                    public const string ClassFileName = @"c:\test\MainProject\BasicClass.cs";
                    public const string ClassFileContents = 
                        @"
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
                        using System.Text;
                        using System.Threading.Tasks;

                        namespace MainProject
                        {
                            public class BasicClass
                            {
                            }
                        }";
                    public const string ClassName = "BasicClass";
                }
            }
        }
    }
}
