using System;
using System.Collections.Generic;
using CopaceticSoftware.CodeGenerator.StarterKit.Infrastructure;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.ExpectedErrors
{
    [TestFixture]
    public class NoPartialClassTest : ExpectedErrorsTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                    using CopaceticSoftware.pMixins.Attributes;

                    namespace Test
                    {
                        public class Mixin
                        {
                            public string MixinMethod()
                            {
                                return ""MixinMethod"";
                            }                           
                        }
                        
                        [pMixin(Mixin = typeof(Mixin))]
                        public class Target{}
                    }
                ";
            }
        }

        protected override Dictionary<string, Func<CodeGenerationError, bool>> ExpectedErrorMessages
        {
            get
            {
                return new Dictionary<string, Func<CodeGenerationError, bool>>
                       {
                           {
                               Strings.WarningNoPartialClassInSourceFile,
                               error => error.Message == Strings.WarningNoPartialClassInSourceFile
                           }
                       };
            
            }
        }
    }
}