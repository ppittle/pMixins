//----------------------------------------------------------------------- 
// <copyright file="AbstractMixinSpec.cs" company="Copacetic Software"> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.NonParameterlessConstructorMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinDoesNotHaveParameterlessConstructor"/>
    ///     <see cref="MixinIsSealedType"/>
    /// 
    /// Look at that! I work with sealed mixins!
    /// </summary>
    public sealed class NonParameterlessConstructorMixin
    {
        private readonly string _name;

        public NonParameterlessConstructorMixin(string name)
        {
            _name = name;
        }

        public string SealedPrettyPrintName()
        {
            return "Sealed - Pretty " + _name;
        }
    }

    public class NonParameterlessProtectedConstructorMixin
    {
        private readonly string _name;

        protected NonParameterlessProtectedConstructorMixin(string name)
        {
            _name = name;
        }

        public string ProtectedConstructorPrettyPrintName()
        {
            return "Protected - Pretty " + _name;
        }

        protected void RandomProtetectedMethod(){}
    }

    [BasicMixin(Target = typeof (NonParameterlessConstructorMixin)
        /*, RequiresInitialization = true -- should know initialization is required*/)]
    public partial class NonParameterlessConstructorMixinSpec
    {
        private readonly string _name;

        public NonParameterlessConstructorMixinSpec(string name)
        {
            _name = name;
        }

        //Required method - Recommend to implement explicitly
        NonParameterlessConstructorMixin INonParameterlessConstructorMixinRequirements.InitializeNonParameterlessConstructorMixin()
        {
            return new NonParameterlessConstructorMixin(_name);
        }

        NonParameterlessProtectedConstructorMixinWrapper INonParameterlessProtectedConstrutorMixinRequirements.InitializeNonParameterlessProtectedConstrutorMixin()
        {
            return new NonParameterlessProtectedConstructorMixinWrapper(_name);
        }
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public interface INonParameterlessConstructorMixinRequirements
    {
        global::CopaceticSoftware.pMixins.TheorySandbox.COVERED.NonParameterlessConstructorMixin.NonParameterlessConstructorMixin 
            InitializeNonParameterlessConstructorMixin();
    }

    public class NonParameterlessProtectedConstructorMixinWrapper : global::CopaceticSoftware.pMixins.TheorySandbox.COVERED.NonParameterlessConstructorMixin.NonParameterlessProtectedConstructorMixin
    {
        //Create a public constructor to wrap protected constructor
        public NonParameterlessProtectedConstructorMixinWrapper(string name) : base(name)
        {
        }

        //Elevate protected members to public
        public void RandomProtectedMethod()
        {
            base.RandomProtetectedMethod();
        }
    }

    public interface INonParameterlessProtectedConstrutorMixinRequirements
    {
        NonParameterlessProtectedConstructorMixinWrapper InitializeNonParameterlessProtectedConstrutorMixin();
    }

    public partial class NonParameterlessConstructorMixinSpec : INonParameterlessConstructorMixinRequirements,
        INonParameterlessProtectedConstrutorMixinRequirements      
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(NonParameterlessConstructorMixinSpec host)
            {
                _NonParamaterlessConstructorMixin = new Lazy<NonParameterlessConstructorMixin>(
                    () =>
                        //Important: Explicit cast so that Target can implement methods explicitly
                        ((INonParameterlessConstructorMixinRequirements)host)
                            .InitializeNonParameterlessConstructorMixin());

                _NonParameterlessProtectedConstrutorMixin = new Lazy<NonParameterlessProtectedConstructorMixinWrapper>(
                    () =>
                        ((INonParameterlessProtectedConstrutorMixinRequirements)host)
                            .InitializeNonParameterlessProtectedConstrutorMixin());
            }

            public readonly Lazy<NonParameterlessConstructorMixin> _NonParamaterlessConstructorMixin;

            public readonly Lazy<NonParameterlessProtectedConstructorMixinWrapper> _NonParameterlessProtectedConstrutorMixin;
        }

        private __Mixins ___mixins;

        private __Mixins __mixins
        {
            get
            {
                if (null == ___mixins)
                    ___mixins = new __Mixins(this);

                return ___mixins;
            }
        }

        public string SealedPrettyPrintName()
        {
            return __mixins._NonParamaterlessConstructorMixin.Value.SealedPrettyPrintName();
        }

        public string ProtectedConstructorPrettyPrintName()
        {
            return ___mixins._NonParameterlessProtectedConstrutorMixin.Value.ProtectedConstructorPrettyPrintName();
        }

        public void RandomProtectedMethod()
        {
            ___mixins._NonParameterlessProtectedConstrutorMixin.Value.RandomProtectedMethod();
        }
    }
}
