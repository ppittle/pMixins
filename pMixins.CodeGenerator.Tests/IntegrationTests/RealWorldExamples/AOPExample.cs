//----------------------------------------------------------------------- 
// <copyright file="AOPExample.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, July 31, 2014 9:57:12 AM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.RealWorldExamples
{
    /// <summary>
    /// AOP Example from Mvc.Recipes.  No additional tests are needed, compilation
    /// is sufficient for this test.
    /// </summary>
    public class AOPExample : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get { return @"

using System;
using System.Linq;
using CopaceticSoftware.pMixins.Attributes;
using CopaceticSoftware.pMixins.Interceptors;
using NUnit.Framework;

namespace pMixins.Mvc.Recipes.AspectOrientedProgramming
{
    public class LoggingAspect : MixinInterceptorBase
    {
        public override void OnTargetInitialized(object sender, InterceptionEventArgs eventArgs)
        {
            Console.WriteLine(""LoggingAspect: Mixin [{0}] initialized for [{1}]"",
                eventArgs.Mixin.GetType().FullName,
                eventArgs.Target.GetType().FullName);
        }

        public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
        {
            Console.WriteLine(
                ""LoggingAspect: [{0}.{1}] called with Parameters: {2}"",
                eventArgs.Target.GetType().FullName,
                eventArgs.MemberName,
                string.Join("","",
                    eventArgs.Parameters.Select(x => 
                        string.Format(""({0}: {1})"", 
                        x.Name, 
                        (x.Value ?? ""<null>"")))));
        }

        public override void OnAfterMethodInvocation(object sender, MethodEventArgs eventArgs)
        {
            if (null != eventArgs.MemberInvocationException)
            {
                Console.WriteLine(
                    ""LoggingAspect: [{0}.{1}] threw an Exception: [{2}]"",
                    eventArgs.Target.GetType().FullName,
                    eventArgs.MemberName,
                    eventArgs.MemberInvocationException.Message);
            }
            else
            {
                Console.WriteLine(
                    ""LoggingAspect: [{0}.{1}] completed by returning [{2}]"",
                    eventArgs.Target.GetType().FullName,
                    eventArgs.MemberName,
                    (null == eventArgs.ReturnType)
                    ? ""<void>""
                    : (null == eventArgs.ReturnValue) ? ""<null>"" : eventArgs.ReturnValue.ToString()
                );
            }
        }
    }

    public class NullParameterAspect : MixinInterceptorBase
    {
        public override void OnBeforeMethodInvocation(object sender, MethodEventArgs eventArgs)
        {
            foreach(var p in eventArgs.Parameters)
                if (null == p.Value)
                    throw new ArgumentNullException(p.Name);
        }
    }


    [pMixin(Mixin = typeof(AspectOrientedProgrammingImpl),
        Interceptors = new[] { typeof(LoggingAspect), typeof(NullParameterAspect) })]
    public partial class AspectOrientedProgramming
    {
        private class AspectOrientedProgrammingImpl
        {
            public string Stringify(int i)
            {
                if (i == 42)
                    throw new Exception(""42 is the Answer to the Universe!"");

                return i.ToString();
            }

            public object ExampleMethod(object o)
            {
                return o;
            }
        }
    }

    [TestFixture]
    public class AspectOrientedProgrammingTest
    {
        [Test]
        public void TestOn1()
        {
            Assert.AreEqual(""1"",
                new AspectOrientedProgramming().Stringify(1));
        }

        [Test]
        public void TestMagicNumber()
        {
            try
            {
                new AspectOrientedProgramming().Stringify(42);

                Assert.Fail(""Expected an Exception to be thrown!"");
            }
            catch (Exception e)
            {
                Assert.True(
                    e.Message.Contains(""42""));
            }
        }

        [Test]
        public void TestNullArgument()
        {
            try
            {
                new AspectOrientedProgramming().ExampleMethod(null);
            }
            catch (Exception e)
            {
                Assert.True(
                    e is ArgumentNullException);
            }
        }
    }
}


            "; }
        }
    }
}
