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

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.AbstractMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinIsAbstractType"/>
    /// </summary>
    public abstract class AbstractMixin
    {
        public abstract string GetName();

        public string PrettyPrintName()
        {
            return "Pretty " + GetName();
        }
    }

    [BasicMixin(Target = typeof (AbstractMixin))]
    public partial class AbstractMixinSpec
    {
        public string GetName()
        {
            return "Hello World!";
        }
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public interface pMixin__TheorySandbox__AbstractMixin__AbstractMixinSpec__IAbstractMixinRequirements
    {
        string GetName();
    }

    public class AbstractMixinWrapper : global::CopaceticSoftware.pMixins.TheorySandbox.COVERED.AbstractMixin.AbstractMixin
    {
        private readonly pMixin__TheorySandbox__AbstractMixin__AbstractMixinSpec__IAbstractMixinRequirements
            _mixinTarget;

        public AbstractMixinWrapper(
            pMixin__TheorySandbox__AbstractMixin__AbstractMixinSpec__IAbstractMixinRequirements target)
        {
            _mixinTarget = target;
        }

        public override string GetName()
        {
            return _mixinTarget.GetName();
        }
    }

    public partial class AbstractMixinSpec : pMixin__TheorySandbox__AbstractMixin__AbstractMixinSpec__IAbstractMixinRequirements
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(AbstractMixinSpec target)
            {
                _AbstractMixin = new Lazy<AbstractMixinWrapper>(
                    () => 
                        new DefaultMixinActivator().CreateInstance<AbstractMixinWrapper>(
                            //Cast to mixin requirements interface in case target has implemented requirements explicitly
                            ((pMixin__TheorySandbox__AbstractMixin__AbstractMixinSpec__IAbstractMixinRequirements)
                                target)));
            }

            public readonly Lazy<AbstractMixinWrapper> _AbstractMixin;
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

        public string PrettyPrintName()
        {
            return __mixins._AbstractMixin.Value.PrettyPrintName();
        }

        public static implicit operator global::CopaceticSoftware.pMixins.TheorySandbox.COVERED.AbstractMixin.AbstractMixin(AbstractMixinSpec target)
        {
            return target.__mixins._AbstractMixin.Value;
        }
    }
}