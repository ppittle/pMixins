//----------------------------------------------------------------------- 
// <copyright file="CSharpFileFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, May 5, 2014 1:41:59 PM</date> 
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.CodeGenerator.StarterKit.Logging;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    public interface ICSharpFileFactory
    {
        CSharpFile BuildCSharpFile(CSharpProject p, FilePath filename);
    }

    public class CSharpFileFactory : ICSharpFileFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IFileReader _fileReader;
        private readonly IVisualStudioOpenDocumentManager _openDocumentManager;

        private static ConcurrentDictionary<FilePath, CSharpFile> _fileCache =
            new ConcurrentDictionary<FilePath, CSharpFile>();

        private FileByProjectIndex _fileByProjectIndex = new FileByProjectIndex();
       

        public CSharpFileFactory(IFileReader fileReader, ICacheEventHelper cacheEventHelper, IVisualStudioEventProxy visualStudioEventProxy, IVisualStudioOpenDocumentManager openDocumentManager)
        {
            _fileReader = fileReader;
            _openDocumentManager = openDocumentManager;

            WireUpCacheEvictionEvents(cacheEventHelper, visualStudioEventProxy);
        }

        public CSharpFile BuildCSharpFile(CSharpProject p, FilePath filename)
        {
            if (_openDocumentManager.IsDocumentOpen(filename))
            {
                _log.InfoFormat("Class file is open and will not be cached: [{0}]", filename);

                return new CSharpFile(p, filename, _fileReader.ReadAllText(filename));
            }

            return _fileCache.GetOrAdd(filename,
                f =>
                {
                    _log.DebugFormat("Build CSharpFile [{0}]", filename);

                    _fileByProjectIndex.Add(p.FileName, f);

                    return new CSharpFile(p, f, _fileReader.ReadAllText(f));
                });
        }

        private void WireUpCacheEvictionEvents(ICacheEventHelper cacheEventHelper, IVisualStudioEventProxy visualStudioEventProxy)
        {
            cacheEventHelper.OnClearCache +=
                (sender, args) =>
                {
                    _log.Info("Solution closing.  Clearing cache");
                    _fileCache = new ConcurrentDictionary<FilePath, CSharpFile>();

                    _fileByProjectIndex = new FileByProjectIndex();
                };

            cacheEventHelper.OnEvictFromCache += 
                (sender, args) =>
                {
                    CSharpFile dummy;

                    if (_fileCache.TryRemove(args.FileName, out dummy))
                        _log.InfoFormat("Evicted [{0}]", args.FileName);
                };
            
            visualStudioEventProxy.OnProjectRemoved +=
                (sender, args) => EvictAllFilesInProject(args.ProjectFullPath);

            //Evict files, their reference to the Project is no longer valid
            visualStudioEventProxy.OnProjectReferenceAdded +=
                (sender, args) => EvictAllFilesInProject(args.ProjectFullPath);

            //Evict files, their reference to the Project is no longer valid
            visualStudioEventProxy.OnProjectReferenceRemoved +=
                (sender, args) => EvictAllFilesInProject(args.ProjectFullPath);
        }

        private void EvictAllFilesInProject(FilePath projectFullPath)
        {
            using (new LoggingActivity("Evicting file for Project [" + projectFullPath + "]"))
            {
                CSharpFile dummy;

                foreach (var fileInProject in _fileByProjectIndex.RemoveProjectFileList(projectFullPath))
                    if (_fileCache.TryRemove(fileInProject, out dummy))
                        _log.InfoFormat("Evicted [{0}]", fileInProject);
            }
        }
    }

    public class FileByProjectIndex
    {
        private Dictionary<FilePath, IList<FilePath>> _cache =
            new Dictionary<FilePath, IList<FilePath>>();

        private static object _lock = new object();

        public void Add(FilePath projectFilePath, FilePath filePath)
        {
            lock (_lock)
            {
                IList<FilePath> value;
                if (_cache.TryGetValue(projectFilePath, out value))
                {
                    value.Add(filePath);
                }
                else
                {
                    _cache.Add(projectFilePath, new List<FilePath> { filePath });
                }
            }
        }

        public IList<FilePath> RemoveProjectFileList(FilePath projetFilePath)
        {
            lock (_lock)
            {
                IList<FilePath> value;
                if (_cache.TryGetValue(projetFilePath, out value))
                {
                    _cache.Remove(projetFilePath);
                    return value;
                }
                else
                {
                    return new FilePath[0];
                }
            }

        }
    }
}
