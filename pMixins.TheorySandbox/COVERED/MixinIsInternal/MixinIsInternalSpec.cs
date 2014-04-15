//----------------------------------------------------------------------- 
// <copyright file="MixinIsInternalSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Thursday, February 20, 2014 3:03:43 PM</date> 
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.MixinIsInternal
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinIsInternalType"/>
    /// </summary>
    internal class InternalClass
    {
        internal string InternalMethod()
        {
            return "Internal";
        }
    }

    [BasicMixin(Target = typeof (InternalClass))]
    public partial class MixinIsInternalSpec
    {
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    [GeneratedCode("pMixin", "1.0.0.0")]
    internal abstract class ProtectedMembersWrapper : InternalClass { }
    
    [GeneratedCode("pMixin", "1.0.0.0")]
    internal class AbstractWrapper : ProtectedMembersWrapper
    {
    }

    [GeneratedCode("pMixin", "1.0.0.0")]
    public sealed class MasterWrapper
    {
        //must be public for static conversions
        private readonly AbstractWrapper AbstractWrapper;

        public MasterWrapper(MixinIsInternalSpec target)
        {
            //MixinAttribute.RequiresInitialization = false, so we can construct
            //AbstractWrapper directly
            AbstractWrapper = new AbstractWrapper();
        }

        public string InternalMethod()
        {
            return AbstractWrapper.InternalMethod();
        }
    }

    public partial class MixinIsInternalSpec
    {
        private sealed class __Mixins
        {
            public static readonly global::System.Object ____Lock = new global::System.Object();

            public readonly MasterWrapper
                InternalClassMasterWrapper;

            public __Mixins(MixinIsInternalSpec target)
            {
                InternalClassMasterWrapper = new DefaultMixinActivator().CreateInstance<MasterWrapper>(target);
            }
        }

        private __Mixins _____mixins;
        private __Mixins ___mixins
        {
            get
            {
                if (null == _____mixins)
                {
                    lock (__Mixins.____Lock)
                    {
                        if (null == _____mixins)
                        {
                            _____mixins = new __Mixins(this);
                        }
                    }
                }
                return _____mixins;
            }
        }

        internal string InternalMethod()
        {
            return ___mixins.InternalClassMasterWrapper.InternalMethod();
        }

        //Can't have implicit operator - Mixin is not public
        //public static implicit operator InternalClass(MixinIsInternalSpec target)
        //{
        //    return target.___mixins.InternalClassMasterWrapper.AbstractWrapper;
        //}
    }
}