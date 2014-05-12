﻿//----------------------------------------------------------------------- 
// <copyright file="pMixinVisualStudioCodeGenerator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, March 8, 2014 10:44:15 PM</date> 
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
using System.Runtime.InteropServices;
using System.Text;
using log4net;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;
using Constants = CopaceticSoftware.pMixins.VisualStudio.Extensions.Constants;

namespace CopaceticSoftware.pMixins_VSPackage
{
    [ComVisible(true)]
    [Guid("3E3CAED9-8C24-4332-A774-059F50FF38D6")]
    [ProvideObject(typeof(pMixinsEmptySingleFileCodeGenerator))]
    [CodeGeneratorRegistration(
        typeof(pMixinsEmptySingleFileCodeGenerator),
        "C# pMixins Code Generator",
        vsContextGuids.vsContextGuidVCSProject,
        GeneratesDesignTimeSource = true)]
    public class pMixinsEmptySingleFileCodeGenerator : IVsSingleFileGenerator
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            try
            {
                pbstrDefaultExtension =  Constants.PMixinFileExtension;
                return VSConstants.S_OK;
            }
            catch (Exception e)
            {
                _log.Error("GetDefaultExtentsions threw an Exception: " + e.Message, e);
                pbstrDefaultExtension = string.Empty;
                return VSConstants.E_FAIL;
            }
        }

        /// <summary>
        /// Implements the IVsSingleFileGenerator.Generate method.
        /// Executes the transformation and returns the newly generated output file, whenever a custom tool is loaded, or the input file is saved
        /// </summary>
        /// <param name="wszInputFilePath">The full path of the input file. May be a null reference (Nothing in Visual Basic) in future releases of Visual Studio, so generators should not rely on this value</param>
        /// <param name="bstrInputFileContents">The contents of the input file. This is either a UNICODE BSTR (if the input file is text) or a binary BSTR (if the input file is binary). If the input file is a text file, the project system automatically converts the BSTR to UNICODE</param>
        /// <param name="wszDefaultNamespace">This parameter is meaningful only for custom tools that generate code. It represents the namespace into which the generated code will be placed. If the parameter is not a null reference (Nothing in Visual Basic) and not empty, the custom tool can use the following syntax to enclose the generated code</param>
        /// <param name="rgbOutputFileContents">[out] Returns an array of bytes to be written to the generated file. You must include UNICODE or UTF-8 signature bytes in the returned byte array, as this is a raw stream. The memory for rgbOutputFileContents must be allocated using the .NET Framework call, System.Runtime.InteropServices.AllocCoTaskMem, or the equivalent Win32 system call, CoTaskMemAlloc. The project system is responsible for freeing this memory</param>
        /// <param name="pcbOutput">[out] Returns the count of bytes in the rgbOutputFileContent array</param>
        /// <param name="pGenerateProgress">A reference to the IVsGeneratorProgress interface through which the generator can report its progress to the project system</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns E_FAIL</returns>
       public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {

            var generatedFileByes = Encoding.UTF8.GetBytes(string.Empty);

            // The contract between IVsSingleFileGenerator implementors and consumers is that 
            // any output returned from IVsSingleFileGenerator.Generate() is returned through  
            // memory allocated via CoTaskMemAlloc(). Therefore, we have to convert the 
            // byte[] array returned from GenerateCode() into an unmanaged blob.  

            int outputLength = generatedFileByes.Length;
            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(outputLength);
            Marshal.Copy(generatedFileByes, 0, rgbOutputFileContents[0], outputLength);
            pcbOutput = (uint)outputLength;
            return VSConstants.S_OK;
        }
    }
}
