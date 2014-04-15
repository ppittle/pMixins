//----------------------------------------------------------------------- 
// <copyright file="MultipleMixinsMethodPrioritySpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, October 16, 2013 1:40:04 PM</date> 
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
using CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostCanOverrideAndExposeVirtualMixinMembers;

namespace CopaceticSoftware.pMixins.TheorySandbox.MultipleMixinsMethodPriority
{
    /// <summary>
    /// The techniques here are tested in <see cref="HostCanOverrideAndExposeVirtualMixinMembersSpec"/>
    /// </summary>
    public abstract class BaseClass
    {
        public void A(){}
        public void B(){}
        public virtual void C(){}
        public abstract void X();
        public virtual void Y(){}
        public virtual void Z(){}
        
    }

    public class MixinOne
    {
        public void A(){}
        public void D(){}
    }

    public class MixinTwo
    {
        public void D(){}
        public void E(){}
        public virtual void F(){}
        public virtual void G(){}
    }

    public class MixinThree : BaseClass
    {
        public override void C()
        {
            
        }

        public override void X(){}
    }

    public class MixinFour : MixinTwo
    {
        public override void F(){}
    }

    public class MixinFive : BaseClass
    {
        public override void X(){}

        public override void Y(){}
    }


    [BasicMixin(Target = typeof(MixinOne))]
    [BasicMixin(Target = typeof(MixinTwo))]
    [BasicMixin(Target = typeof(MixinThree))]
    [BasicMixin(Target = typeof(MixinFour))]
    [BasicMixin(Target = typeof(MixinFive))]
    public partial class MultipleMixinsMethodPrioritySpec : BaseClass
    {
        
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public interface IVirtualMembersShim
    {
        void C();
        void F();
        void G();
        void X();
        void Y();
    }

    public class MixinThreeWrapper : MixinThree
    {
        private IVirtualMembersShim _host;

        public MixinThreeWrapper(IVirtualMembersShim host)
        {
            _host = host;

            CFunc = () => base.C();
            XFunc = () => base.X();
            YFunc = () => base.Y();
        }

        public Action CFunc { get; set; }
        public Action XFunc { get; set; }
        public Action YFunc { get; set; }

        public override void C()
        {
            _host.C();
        }

        public override void X()
        {
            _host.X();
        }

        public override void Y()
        {
            _host.Y();
        }
    }

    public class MixinFourWrapper : MixinFour
    {
        private IVirtualMembersShim _host;

        public MixinFourWrapper(IVirtualMembersShim host)
        {
            _host = host;

            FFunc = () => base.F();
            GFunc = () => base.G();
        }

        public Action FFunc { get; set; }
        public Action GFunc { get; set; }

        public override void F()
        {
            _host.F();
        }

        public override void G()
        {
            _host.G();
        }
    }

    public class MixinFiveWrapper : MixinFive
    {
        private IVirtualMembersShim _host;

        public MixinFiveWrapper(IVirtualMembersShim host)
        {
            _host = host;

            CFunc = () => base.C();
            XFunc = () => base.X();
            YFunc = () => base.Y();
        }

        public Action CFunc { get; set; }
        public Action XFunc { get; set; }
        public Action YFunc { get; set; }

        public override void C()
        {
            _host.C();
        }

        public override void X()
        {
            _host.X();
        }

        public override void Y()
        {
            _host.Y();
        }
    }

    public partial class MultipleMixinsMethodPrioritySpec : IVirtualMembersShim
    {
        #region Define Mixins Class and Property
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(MultipleMixinsMethodPrioritySpec host)
            {
                _MixinOneMixin = new Lazy<MixinOne>(
                    () => new DefaultMixinActivator().CreateInstance<MixinOne>());

                // _MixinTwoMixin = new Lazy<MixinTwo>(
                //     () => new DefaultMixinActivator().CreateInstance<MixinTwo>());

                _MixinThreeMixin = new Lazy<MixinThreeWrapper>(
                    () => new DefaultMixinActivator().CreateInstance<MixinThreeWrapper>(this));

                _MixinFourMixin = new Lazy<MixinFourWrapper>(
                    () => new DefaultMixinActivator().CreateInstance<MixinFourWrapper>(this));

                _MixinFiveMixin = new Lazy<MixinFiveWrapper>(
                    () => new DefaultMixinActivator().CreateInstance<MixinFiveWrapper>(this));
            }

            public readonly Lazy<MixinOne> _MixinOneMixin;
            //Mixin two is ignored because mixin four is a child 
            //public readonly Lazy<MixinTwo> _MixinTwoMixin;
            public readonly Lazy<MixinThreeWrapper> _MixinThreeMixin;
            public readonly Lazy<MixinFourWrapper> _MixinFourMixin;
            public readonly Lazy<MixinFiveWrapper> _MixinFiveMixin;
        }

        private MultipleMixinsMethodPrioritySpec.__Mixins ___mixins;

        private MultipleMixinsMethodPrioritySpec.__Mixins __mixins
        {
            get
            {
                if (null == ___mixins)
                    ___mixins = new MultipleMixinsMethodPrioritySpec.__Mixins(this);

                return ___mixins;
            }
        }
        #endregion

        /// <summary>
        /// <see cref="MixinThree"/> overrides the base class. 
        /// </summary>
        public override void C()
        {
            __mixins._MixinThreeMixin.Value.CFunc();
        }

        /// <summary>
        /// Use <see cref="MixinOne.D"/> as it has
        /// higher precedence then <see cref="MixinTwo.D"/>
        /// because it was Mixed in first.
        /// </summary>
        public void D()
        {
            __mixins._MixinOneMixin.Value.D();
        }

        /// <summary>
        /// No conflicts
        /// </summary>
        public void E()
        {
            __mixins._MixinFourMixin.Value.E();
        }

        public virtual void F()
        {
            __mixins._MixinFourMixin.Value.FFunc();
        }

        public virtual void G()
        {
            __mixins._MixinFourMixin.Value.GFunc();
        }

        /// <summary>
        /// Spec's base class has an abstract method
        /// that is satisfied by <see cref="MixinThree" />.
        /// <see cref="MixinFive.X"/> is dropped.
        /// </summary>
        public override void X()
        {
            __mixins._MixinThreeMixin.Value.XFunc();
        }
        
        /// <summary>
        /// Spec's base class has a virtual
        /// method that is overriden in <see cref="MixinFive"/>,
        /// </summary>
        public override void Y()
        {
            __mixins._MixinFiveMixin.Value.YFunc();
        }

        /// <summary>
        /// No Mixin's override this
        /// method so don't do anything with it.
        /// </summary>
        //public override void Z()
        //{
        //    base.Z();
        //}

        #region Conversion Methods
        public static implicit operator MixinOne(MultipleMixinsMethodPrioritySpec host)
        {
            return host.__mixins._MixinOneMixin.Value;
        }

        //public static implicit operator MixinTwo(MultipleMixinsMethodPrioritySpec host)
        //{
        //    return host.__mixins._MixinTwoMixin.Value;
        //}

        public static implicit operator MixinThree(MultipleMixinsMethodPrioritySpec host)
        {
            return host.__mixins._MixinThreeMixin.Value;
        }

        public static implicit operator MixinFour(MultipleMixinsMethodPrioritySpec host)
        {
            return host.__mixins._MixinFourMixin.Value;
        }

        public static implicit operator MixinFive(MultipleMixinsMethodPrioritySpec host)
        {
            return host.__mixins._MixinFiveMixin.Value;
        }
        #endregion
    }
}