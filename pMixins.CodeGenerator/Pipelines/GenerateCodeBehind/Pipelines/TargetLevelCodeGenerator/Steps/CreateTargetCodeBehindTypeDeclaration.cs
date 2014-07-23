//----------------------------------------------------------------------- 
// <copyright file="CreateTargetCodeBehindTypeDeclaration.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, July 23, 2014 11:34:24 AM</date> 
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
using CopaceticSoftware.Common.Patterns;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCodeBehind.Pipelines.TargetLevelCodeGenerator.Steps
{
    /// <summary>
    /// Creates the Target's class declaration for the code behind file and 
    /// saves it in <see cref="TargetLevelCodeGeneratorPipeline.TargetCodeBehindTypeDeclaration"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// //Source:
    /// [pMixin(Mixin = typeof(Mixin)]
    /// public partial class Target{}
    /// 
    /// //Generates:
    /// public partial class Target{}
    /// ]]>
    /// </code>
    /// </example>
    public class CreateTargetCodeBehindTypeDeclaration : IPipelineStep<TargetLevelCodeGeneratorPipeline>
    {
        public bool PerformTask(TargetLevelCodeGeneratorPipeline manager)
        {
            throw new NotImplementedException();
        }
    }
}
