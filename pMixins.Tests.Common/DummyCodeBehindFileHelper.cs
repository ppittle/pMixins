//----------------------------------------------------------------------- 
// <copyright file="DummyCodeBehindFileHelper.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, May 11, 2014 5:14:06 PM</date> 
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

using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.pMixins.VisualStudio.IO;

namespace CopaceticSoftware.pMixins.Tests.Common
{
    public class DummyCodeBehindFileHelper : ICodeBehindFileHelper
    {
        public FilePath GetOrAddCodeBehindFile(FilePath classFileName)
        {
            return new FilePath(
                    classFileName.FullPath.ToLower().Replace(".cs", ".mixin.cs"));
        }
    }
}
