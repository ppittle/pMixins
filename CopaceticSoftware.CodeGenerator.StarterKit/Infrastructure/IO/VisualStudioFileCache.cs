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
using System.Threading.Tasks;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO
{
    public interface IFileReader
    {
        string ReadAllText(string filename);
        IEnumerable<string> ReadLines(string filename);
    }

    public class VisualStudioFileCache : IFileReader
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ConcurrentDictionary<string, FileReaderAsync> _fileCache = 
            new ConcurrentDictionary<string, FileReaderAsync>();

       public VisualStudioFileCache(IVisualStudioEventProxy visualStudioEventProxy)
        {
            WireUpCacheEvictionEvents(visualStudioEventProxy);
        }

        public string ReadAllText(string filename)
        {
            return 
                _fileCache.GetOrAdd(filename, f => new FileReaderAsync(f))
                    .FileContents;
        }

        public IEnumerable<string> ReadLines(string filename)
        {
            return ReadAllText(filename)
                .Split(new [] {Environment.NewLine}, StringSplitOptions.None);
        }

        private void WireUpCacheEvictionEvents(IVisualStudioEventProxy visualStudioEventProxy)
        {
            FileReaderAsync dummy;

            visualStudioEventProxy.OnSolutionClosing +=
                (sender, args) =>
                {
                    _log.Info("Solution closing.  Clearing cache");
                    _fileCache = new ConcurrentDictionary<string, FileReaderAsync>();
                };

            visualStudioEventProxy.OnProjectRemoved +=
                (sender, args) =>
                {
                    if (_fileCache.TryRemove(args.ProjectFullPath, out dummy))
                        _log.InfoFormat("Evicted [{0}]", args.ProjectFullPath);
                }; 

            visualStudioEventProxy.OnProjectItemAdded +=
                (sender, args) =>
                {
                    if ((Path.GetExtension(args.ClassFullPath) ?? "").ToLower().Equals(".cs"))
                    {
                        _log.InfoFormat("Eagerly adding file to cache [{0}]", args.ClassFullPath);
                        ReadAllText(args.ClassFullPath);
                    }
                };

            visualStudioEventProxy.OnProjectItemSaved +=
                (sender, args) =>
                {
                    if (_fileCache.TryRemove(args.ClassFullPath, out dummy))
                    {
                        _log.InfoFormat("Evicted [{0}]", args.ClassFullPath);

                        ReadAllText(args.ClassFullPath);
                    }
                };

            visualStudioEventProxy.OnProjectItemRemoved +=
                (sender, args) =>
                {
                    if (_fileCache.TryRemove(args.ClassFullPath, out dummy))
                        _log.InfoFormat("Evicted [{0}]", args.ClassFullPath);
                };

            visualStudioEventProxy.OnProjectItemRenamed +=
                (sender, args) =>
                {
                    if (_fileCache.TryRemove(args.OldClassFileName, out dummy))
                    {
                        _log.InfoFormat("Evicted [{0}]", args.OldClassFileName);

                        ReadAllText(args.ClassFullPath);
                    }
                };

            visualStudioEventProxy.OnProjectItemOpened +=
                (sender, args) =>
                {
                    FileReaderAsync fileReader;

                    if (_fileCache.TryGetValue(args.ClassFullPath, out fileReader))
                    {
                        _log.InfoFormat("Document Opened [{0}]", args.ClassFullPath);

                        fileReader.FileIsOpenInEditor(args.DocumentReader);
                    }
                };

            visualStudioEventProxy.OnProjectItemClosed +=
                (sender, args) =>
                {
                    FileReaderAsync fileReader;

                    if (_fileCache.TryGetValue(args.ClassFullPath, out fileReader))
                    {
                        _log.InfoFormat("Document Opened [{0}]", args.ClassFullPath);

                        fileReader.FileIsCloseedInEditor();
                    }
                };
            
        }
    }

    public class FileReaderAsync
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Task _readFileTask;

        private Exception _fileReadException = null;

        private string _fileContents;

        private string _fileName;

        private IVisualStudioOpenDocumentReader _openDocumentReader = null;

        public FileReaderAsync(string filename)
        {
            _fileName = filename;

            _readFileTask = new TaskFactory().StartNew(() =>
            {
                try
                {
                    _log.DebugFormat("Reading File [{0}]", filename);

                    _fileContents = File.ReadAllText(filename);
                }
                catch (Exception e)
                {
                    _fileReadException = e;
                }
            });
        }

        public void FileIsOpenInEditor(IVisualStudioOpenDocumentReader openDocumentReader)
        {
            _openDocumentReader = openDocumentReader;
        }

        public void FileIsCloseedInEditor()
        {
            _openDocumentReader = null;
        }

        public string FileContents
        {
            get
            {
                if (null != _openDocumentReader)
                {
                    _log.InfoFormat("Document is open in editor [{0}]", _fileName);

                    return _openDocumentReader.GetDocumentText();
                }

                _readFileTask.Wait();

                if (null != _fileReadException)
                    throw _fileReadException;

                return _fileContents;
            }
        }
    }
}
