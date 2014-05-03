//----------------------------------------------------------------------- 
// <copyright file="VisualStudioCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, May 3, 2014 3:15:31 PM</date> 
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
using System.Reflection;
using System.Text;
using CopaceticSoftware.CodeGenerator.StarterKit;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using log4net;

namespace CopaceticSoftware.pMixins.VisualStudio
{
    public interface IVisualStudioCodeGenerator
    {
        byte[] GenerateCode(string inputFileContent, string fileName, string projectFileName);
    }

    public class VisualStudioCodeGenerator : IVisualStudioCodeGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IVisualStudioWriter _visualStudioWriter;
        private readonly ICodeGeneratorContextFactory _codeGeneratorContextFactory;
        private readonly IPartialCodeGenerator _codeGenerator;

        public VisualStudioCodeGenerator(IVisualStudioWriter visualStudioWriter, IpMixinsSolutionManager solutionManager, IPartialCodeGenerator codeGenerator, ICodeGeneratorContextFactory codeGeneratorContextFactory)
        {
            _visualStudioWriter = visualStudioWriter;
            _codeGenerator = codeGenerator;
            _codeGeneratorContextFactory = codeGeneratorContextFactory;
        }

        public byte[] GenerateCode(string inputFileContent, string fileName, string projectFileName)
        {
            inputFileContent = inputFileContent ?? "";
            fileName = fileName ?? "";
            projectFileName = projectFileName ?? "";
            
            Log.InfoFormat("Generating Code for file [{0}] in [{1}]",
                    fileName, projectFileName);

            Log.DebugFormat("Input File Contents: {0}{1}",
                Environment.NewLine,
                inputFileContent);

            try
            {
                var context = _codeGeneratorContextFactory.GenerateContext(inputFileContent, fileName, projectFileName);

                var response = _codeGenerator.GeneratePartialCode(context);

                #region Write Errors / Warnings

                foreach (var error in response.Errors)
                    switch (error.Severity)
                    {
                        case CodeGenerationError.SeverityOptions.Error:
                            _visualStudioWriter.GeneratorError(error.Message, error.Line, error.Column);
                            break;

                        case CodeGenerationError.SeverityOptions.Warning:
                            _visualStudioWriter.GeneratorWarning(error.Message, error.Line, error.Column);
                            break;

                        case CodeGenerationError.SeverityOptions.Message:
                            _visualStudioWriter.GeneratorMessage(error.Message, error.Line, error.Column);
                            break;
                    }

                #endregion

                var generatedCode = response.GeneratedCodeSyntaxTree.GetText();

                Log.InfoFormat("Generated Code for File [{0}]: {1}{2}{1}",
                    Environment.NewLine,
                    generatedCode);

                return Encoding.UTF8.GetBytes(generatedCode);
            }
            catch (Exception e)
            {
                var errorMessage =
                    string.Format("Unhandled Exception Generating Code for File [{0}] in Product [{1}]: {2}",
                        fileName,
                        projectFileName,
                        e.Message);

                if (Log.IsInfoEnabled)
                    errorMessage += Environment.NewLine + "File Contents:" + Environment.NewLine + inputFileContent;

                Log.Error(errorMessage, e);

                return new byte[0];
            }
        }
    }
}
