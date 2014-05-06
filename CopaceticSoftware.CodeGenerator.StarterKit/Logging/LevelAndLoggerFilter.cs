//----------------------------------------------------------------------- 
// <copyright file="LevelAndLoggerFilter.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, May 6, 2014 8:29:04 PM</date> 
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

using log4net.Core;
using log4net.Filter;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Logging
{
    public class LevelAndLoggerFilter : IFilter
    {
        public string LoggerToMatch { get; set; }
        public Level LevelMax { get; set; }
        public Level LevelMin { get; set; }

        public LevelAndLoggerFilter()
        {
            LoggerToMatch = "";
            LevelMax = Level.Fatal;
            LevelMin = Level.All;
        }

        public void ActivateOptions() { }

        public FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (!(loggingEvent.LoggerName.StartsWith(LoggerToMatch)))
                return FilterDecision.Accept;

            return (loggingEvent.Level >= LevelMin &&
                    loggingEvent.Level <= LevelMax)
                ? FilterDecision.Accept
                : FilterDecision.Deny;
        }

        public IFilter Next { get; set; }
    }
}
