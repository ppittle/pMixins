//----------------------------------------------------------------------- 
// <copyright file="pMixinPartialCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, November 10, 2013 12:26:14 AM</date> 
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

using System.Diagnostics;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines;
using log4net;

namespace CopaceticSoftware.pMixins.CodeGenerator
{
    public class pMixinPartialCodeGenerator : IPartialCodeGenerator
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public pMixinPartialCodeGeneratorResponse GeneratePartialCode(ICodeGeneratorContext codeGeneratorContext)
        {
            var stopwatch = Stopwatch.StartNew();

            var logManager = new Log4NetInMemoryStreamAppenderManager();

            var pipelineState = new CodeGenerationPipelineState(codeGeneratorContext);

            new pMixinPartialCodeGeneratorPipeline().PerformTask(pipelineState);

            return new pMixinPartialCodeGeneratorResponse
                       {
                           CodeGeneratorExecutionTime = stopwatch.Elapsed,
                           CodeGeneratorPipelineState = pipelineState,
                           Errors = pipelineState.CodeGenerationErrors,
                           GeneratedCodeSyntaxTree = pipelineState.GeneratedCodeSyntaxTree,
                           LogMessages = logManager.GetRenderedLoggingEvents(LoggingVerbosity.All)
                       };
        }

        CodeGeneratorResponse IPartialCodeGenerator.GeneratePartialCode(ICodeGeneratorContext codeGeneratorContext)
        {
            return GeneratePartialCode(codeGeneratorContext);
        }
    }



    public class pMixinPartialCodeGeneratorResponse : CodeGeneratorResponse
    {
        public CodeGenerationPipelineState CodeGeneratorPipelineState { get; set; }
    }
}
