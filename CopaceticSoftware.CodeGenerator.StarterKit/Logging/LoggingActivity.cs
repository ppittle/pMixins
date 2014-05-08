//----------------------------------------------------------------------- 
// <copyright file="LoggingActivity.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 10:09:15 PM</date> 
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
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    /// <summary>
    /// Adds some entry / exit text to the output window
    /// to help read the Output Window
    /// </summary>
    public class LoggingActivity : IDisposable
    {
        private static IVisualStudioWriter _staticVisualStudioWriter;

        public static void Initialize(IVisualStudioWriter visualStudioWriter)
        {
            _staticVisualStudioWriter = visualStudioWriter;
        }

        private readonly string _activityName;

        private readonly Stopwatch _sw = Stopwatch.StartNew();

        private readonly IVisualStudioWriter _visualStudioWriter;

        public LoggingActivity(string activityName, IVisualStudioWriter visualStudioWriter = null)
        {
            _activityName = activityName;

            _visualStudioWriter = visualStudioWriter ?? _staticVisualStudioWriter;

            if (null == _visualStudioWriter)
                throw new Exception("Both _staticVisualStudioWriter and visualStudioWriter are null.  Call LoggingActivity.Initialize when creating the Kernel.");

            _visualStudioWriter.OutputString("\r\n\r\n        --- [" + activityName + "] BEGIN ---  \r\n");
        }
        
        public void Dispose()
        {
            _visualStudioWriter.OutputString(
                string.Format(
                    "        --- [{0}] COMPLETE [{1}] ms ---  \r\n\r\n",
                    _activityName,
                    _sw.ElapsedMilliseconds));
        }
    }
}
