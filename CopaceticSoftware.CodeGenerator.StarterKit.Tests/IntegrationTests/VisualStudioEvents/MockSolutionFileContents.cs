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

using System.IO;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Tests.IntegrationTests.VisualStudioEvents
{
    public static class MockSolutionFileContents
    {
        public static string GetReferenceToDllInTestProject(string dllName)
        {
            return 
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        dllName));
        }

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
                      <HintPath>" + GetReferenceToDllInTestProject("CopaceticSoftware.pMixins.dll") +@"</HintPath>
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
                      <HintPath>" + GetReferenceToDllInTestProject("CopaceticSoftware.pMixins.dll") + @"</HintPath>
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
