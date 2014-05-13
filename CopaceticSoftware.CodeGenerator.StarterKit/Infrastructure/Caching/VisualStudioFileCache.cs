//----------------------------------------------------------------------- 
// <copyright file="VisualStudioFileCache.cs" company="Copacetic Software"> 
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.Caching
{
    public interface IFileReader
    {
        string ReadAllText(string filename);
        IEnumerable<string> ReadLines(string filename);

        void EvictFromCache(string filename);
    }

    public class VisualStudioFileCache : IFileReader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentDictionary<string, FileReaderAsync> _fileCache = 
            new ConcurrentDictionary<string, FileReaderAsync>();

        private readonly IFileWrapper _fileWrapper;
        private readonly IVisualStudioOpenDocumentManager _openDocumentManager;

        public VisualStudioFileCache(ICacheEventHelper cacheEventHelper, IVisualStudioEventProxy visualStudioEventProxy, IFileWrapper fileWrapper, IVisualStudioOpenDocumentManager openDocumentManager)
        {
            _fileWrapper = fileWrapper;
            _openDocumentManager = openDocumentManager;

            WireUpCacheEvictionEvents(cacheEventHelper, visualStudioEventProxy);
        }

        public string ReadAllText(string filename)
        {
            return 
                _fileCache.GetOrAdd(filename, f => new FileReaderAsync(_openDocumentManager, _fileWrapper, f))
                    .FileContents;
        }

        public IEnumerable<string> ReadLines(string filename)
        {
            return ReadAllText(filename)
                .Split(new [] {Environment.NewLine}, StringSplitOptions.None);
        }

        public void EvictFromCache(string filename)
        {
            FileReaderAsync dummy;

            if (_fileCache.TryRemove(filename, out dummy))
                _log.InfoFormat("Evicted [{0}]", filename);
        }

        private void WireUpCacheEvictionEvents(ICacheEventHelper cacheEventHelper, IVisualStudioEventProxy visualStudioEventProxy)
        {
            cacheEventHelper.OnEvictFromCache += (sender, args) =>
            {
                if (!args.FileOnDiskHasChanged)
                    return;

                FileReaderAsync dummy;

                if (_fileCache.TryRemove(args.FileName, out dummy))
                    _log.InfoFormat("Evicted [{0}]", args.FileName);
            };

            cacheEventHelper.OnClearCache += (sender, args) =>
            {
                _fileCache = new ConcurrentDictionary<string, FileReaderAsync>();

                _log.InfoFormat("Cleared Cache");
            };

            visualStudioEventProxy.OnProjectItemAdded +=
                (sender, args) => TryEagerlyLoadFile(args.ClassFullPath);

            visualStudioEventProxy.OnProjectItemRenamed +=
                (sender, args) => TryEagerlyLoadFile(args.ClassFullPath);
        }

        private void TryEagerlyLoadFile(string filename)
        {
            if ((Path.GetExtension(filename) ?? "").ToLower().Equals(".cs"))
            {
                _log.InfoFormat("Eagerly adding file to cache [{0}]", filename);
                ReadAllText(filename);
            }
        }
    }
}
