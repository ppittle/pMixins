//----------------------------------------------------------------------- 
// <copyright file="TestBase.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, October 16, 2013 1:40:05 PM</date> 
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

using System.Reflection;
using log4net;
using NBehave.Spec.NUnit;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests
{
    public abstract class TestBase : SpecBase
    {
        protected ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected TestBase()
        {
            Log4NetInitializer.Initialize();
        }

    }
    
    public static class Log4NetInitializer
    {
        static Log4NetInitializer()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void Initialize()
        {
        }
    }
}
