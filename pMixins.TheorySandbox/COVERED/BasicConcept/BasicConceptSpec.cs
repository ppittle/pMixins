//----------------------------------------------------------------------- 
// <copyright file="BasicConceptSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, August 22, 2013 10:04:30 PM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.BasicConcept
{   
    /// <summary>
    /// Covered in:
    ///  <see cref="MixinPublicMethodsAreInjectedIntoTarget"/>
    ///  <see cref="StaticImplicitConversionOperator"/>
    /// </summary>
    public class ExampleMixin
    {
        public string Foo()
        {
            return "Foo";
        }
    }

    [BasicMixin(Target = typeof (ExampleMixin))]
    public partial class BasicConceptSpec{}

/*/////////////////////////////////////////
    /// Generated Code
    /////////////////////////////////////////*/

    public partial class BasicConceptSpec
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public readonly Lazy<ExampleMixin> _ExampleMixin = new Lazy<ExampleMixin>(
                () => new DefaultMixinActivator().CreateInstance<ExampleMixin>());
        }

        private readonly __Mixins __mixins = new __Mixins();
        
        public string Foo()
        {
            return __mixins._ExampleMixin.Value.Foo();
        }

        public static implicit operator ExampleMixin(BasicConceptSpec spec)
        {
            return spec.__mixins._ExampleMixin.Value;
        }
    }
}