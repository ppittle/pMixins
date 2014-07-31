//----------------------------------------------------------------------- 
// <copyright file="MeasureMethodExecutionTimeWithAStatefulInterceptor.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 31, 2014 10:50:53 AM</date> 
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

using CopaceticSoftware.pMixins.Interceptors;
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.Interceptors
{
    /// <summary>
    /// Tests if the same Interceptor instance is preserved
    /// between <see cref="IMixinInterceptor.OnBeforeMethodInvocation"/>
    /// and
    /// <see cref="IMixinInterceptor.OnAfterMethodInvocation"/>
    /// </summary>
    [TestFixture]
    public class MeasureMethodExecutionTimeWithAStatefulInterceptor : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get { 
                return @"
                using System;
                using System.Diagnostics;
                using System.Threading;
                using CopaceticSoftware.pMixins.Attributes;
                using CopaceticSoftware.pMixins.Interceptors;

                namespace Test{
                
                public class PerformanceMeasurement : MixinInterceptorBase
                {
                    private Stopwatch _stopwatch;

                    public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
                    {
                        _stopwatch = Stopwatch.StartNew();
                    }
        
                    public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
                    {
                        if (null == _stopwatch)
                            throw new Exception ("" OnBeforeMethodInvocation was not called on this instance! "");
                    
                        _stopwatch.Stop();

                        eventArgs.ReturnValue = _stopwatch.ElapsedMilliseconds;
                    }
                }


                public class Mixin
                {
                    public long GetMethodExecutionTime()
                    {
                        //Sleep for 5 ms
                        Thread.Sleep(5);

                        return 0;
                    }
                }

                [pMixin(Mixin = typeof(Mixin), Interceptors = new []{typeof(PerformanceMeasurement)})]
                public partial class Target
                {
                }

                }
                "; }
        }

        [Test]
        public void CanGetMethodExecutionTime()
        {
            CompilerResults
                .ExecuteMethod<long>(
                    "Test.Target",
                    "GetMethodExecutionTime")

                .ShouldBeGreaterThan(0);
        }
    }
}
