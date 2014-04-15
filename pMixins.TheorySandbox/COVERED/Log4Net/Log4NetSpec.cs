//----------------------------------------------------------------------- 
// <copyright file="Log4NetSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, November 9, 2013 7:39:31 PM</date> 
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;
using log4net;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.Log4Net
{
    /// <summary>
    /// Covered in:
    ///     <see cref="CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests.Log4NetMixin"/>
    /// </summary>
    public class Log4NetMixin
    {
        protected ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }

    [BasicMixin(Target = typeof(Log4NetMixin))]
    public partial class Log4NetSpec
    {
        public void MethodThatLogs()
        {
            Log.Info("Hello World!");
        }
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public class Log4NetMixinWrapper : Log4NetMixin 
    {
        public new ILog Log
        {
            get { return base.Log; }
            set { base.Log = value; }
        }
    }

    public partial class Log4NetSpec
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public readonly Lazy<Log4NetMixinWrapper> _Log4NetMixin = new Lazy<Log4NetMixinWrapper>(
                () => new DefaultMixinActivator().CreateInstance<Log4NetMixinWrapper>());
        }

        private readonly __Mixins __mixins = new __Mixins();

        protected ILog Log
        {
            get { return __mixins._Log4NetMixin.Value.Log; }
            set { __mixins._Log4NetMixin.Value.Log = value; }
        }

        public static implicit operator Log4NetMixin(Log4NetSpec spec)
        {
            return spec.__mixins._Log4NetMixin.Value;
        }
    }
}