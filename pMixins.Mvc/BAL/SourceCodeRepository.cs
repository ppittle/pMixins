//----------------------------------------------------------------------- 
// <copyright file="SourceCodeRepository.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, June 25, 2014 7:00:33 PM</date> 
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
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using ICSharpCode.NRefactory.CSharp;
using log4net;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;

namespace CopaceticSoftware.pMixins.Mvc.BAL
{
    public interface ISourceCodeRepository
    {
        string GetSourceCodeForFile(string pMixinsRecipesFile, string className);
    }

    public class SourceCodeRepository : ISourceCodeRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CSharpParser _parser = new CSharpParser();
        private readonly Regex spaceParanthCleanupRegex = new Regex(@"(?:[a-z. ])( \()");

        private static readonly ConcurrentDictionary<string, string> _fileCache = 
            new ConcurrentDictionary<string, string>();

        private static string PMixinsRecipesZipFilePath;

        private static readonly object _lock = new object();

        public static void Initialize(HttpServerUtility server)
        {
            lock (_lock)
            {
                if (null != PMixinsRecipesZipFilePath)
                    return;

                if (null == server)
                    throw new ArgumentNullException("server");

                PMixinsRecipesZipFilePath = server.MapPath(
                    @"/Content/pMixins.Mvc.Recipes.zip");
            }
        }

        public string GetSourceCodeForFile(string pMixinsRecipesFile, string className)
        {
            if (!pMixinsRecipesFile.StartsWith("pMixins.Mvc.Recipes/"))
                pMixinsRecipesFile = "pMixins.Mvc.Recipes/" + pMixinsRecipesFile;

            var fileContents = GetSourceFile(pMixinsRecipesFile);

            if (string.IsNullOrEmpty(fileContents))
            {
                _log.InfoFormat("Could not read file at [{0}]", pMixinsRecipesFile);
                return string.Empty;
            }

            var syntaxTree = _parser.Parse(fileContents);

            return
                syntaxTree.Descendants.OfType<TypeDeclaration>()
                    .Where(x => x.Name.Equals(className, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.GetText().Trim())
                    //I HATE REGEX
                    //.Select(x => spaceParanthCleanupRegex.Replace(x, "("))
                    .Select(x => x.Replace(" (", "(").Replace("=(", "= ("))
                    .FirstOrDefault();
        }

        private static readonly object zipFileStreamLock = new object();
        private string GetSourceFile(string pMixinsRecipesFile)
        {
            return 
                _fileCache.GetOrAdd(
                    pMixinsRecipesFile,
                    p =>
                    {
                        lock (zipFileStreamLock)
                        {
                            using (var fs = File.Open(PMixinsRecipesZipFilePath, FileMode.Open))
                            using (var zip = new ZipArchive(fs))
                            {
                                var entry = zip.GetEntry(p);

                                using (var entryStream = entry.Open())
                                using (var sr = new StreamReader(entryStream))
                                    return sr.ReadToEnd();

                            }
                        }
                    });
        }
    }
}
