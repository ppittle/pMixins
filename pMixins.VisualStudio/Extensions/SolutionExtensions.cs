//----------------------------------------------------------------------- 
// <copyright file="SolutionExtensions.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 1:06:27 PM</date> 
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
using System.Collections.Generic;
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution;
using CopaceticSoftware.pMixins.Attributes;

namespace CopaceticSoftware.pMixins.VisualStudio.Extensions
{
    public static class SolutionExtensions
    {
        public static IEnumerable<CSharpFile> GetValidPMixinFiles(this Solution s)
        {
            return s.AllFiles
                    .Where(f =>
                        !f.FileName.FullPath.EndsWith(Constants.PMixinFileExtension, 
                            StringComparison.InvariantCultureIgnoreCase) &&
                        f.SyntaxTree.GetPartialClasses().Any(
                        c =>
                        {
                            var resolvedClass = f.CreateResolver().Resolve(c);

                            if (resolvedClass.IsError)
                                return false;

                            return
                                resolvedClass.Type.GetAttributes()
                                    .Any(x => x.AttributeType.Implements<IPMixinAttribute>());
                        }));
        }
    }
}
