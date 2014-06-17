//----------------------------------------------------------------------- 
// <copyright file="SolutionFactory.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, April 30, 2014 11:07:08 PM</date> 
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.IO;
using CopaceticSoftware.Common.Infrastructure;
using JetBrains.Annotations;
using log4net;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure.VisualStudioSolution
{
    public interface ISolutionFactory
    {
        [CanBeNull]
        Solution BuildCurrentSolution();
    }

    public class SolutionFactory : ISolutionFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISolutionFileReader _solutionFileReader;
        private readonly ICSharpProjectFactory _cSharpProjectFactory;
        private readonly ISolutionContext _solutionContext;
        private readonly IVisualStudioWriter _visualStudioWriter;

        public SolutionFactory(ISolutionFileReader solutionFileReader, ICSharpProjectFactory cSharpProjectFactory, ISolutionContext solutionContext, IVisualStudioWriter visualStudioWriter)
        {
            _solutionFileReader = solutionFileReader;
            _cSharpProjectFactory = cSharpProjectFactory;
            _solutionContext = solutionContext;
            _visualStudioWriter = visualStudioWriter;
        }

        public Solution BuildCurrentSolution()
        {
            _visualStudioWriter.WriteToStatusBar("pMixin - Building Current Solution");

            if (_solutionContext.SolutionFileName.IsNullOrEmpty())
            { 
                _log.Warn("There is not a valid Solution in the Solution Context (_solutionContext.SolutionFileName is null or empty).  Returning a null Solution");
                return null;
            }

            var sw = Stopwatch.StartNew();
            int projectCount = 0;
            try
            {
                _log.InfoFormat("Start BuildCurrentSolution on [{0}]", _solutionContext.SolutionFileName);

                var solution = 
                    new Solution(
                        _solutionContext.SolutionFileName,
                        _solutionFileReader.ReadProjectReferences(_solutionContext.SolutionFileName)
                            .Select(pr => _cSharpProjectFactory.BuildProject(pr.ProjectFileName, pr.Title))
                        );

                if (null != solution)
                    projectCount = solution.Projects.Count;

                return solution;
            }
            catch (Exception e)
            {
                _log.Error("Unhandled Exception: " + e.Message, e);

                throw;
            }
            finally
            {
                _log.InfoFormat("BuildCurrentSolution completed with [{0}] Projects in [{1}] ms", 
                    projectCount,
                    sw.ElapsedMilliseconds);

                _visualStudioWriter.WriteToStatusBar("pMixin - Building Current Solution ... Complete");
            }
        }
    }
}
