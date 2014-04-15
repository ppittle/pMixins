//----------------------------------------------------------------------- 
// <copyright file="GenericDecorator.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, March 15, 2014 7:04:18 PM</date> 
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

namespace CopaceticSoftware.pMixins.TheorySandbox.GenericDecorator
{
    public class GenericMixinAttribute : Attribute
    {
        //Is this necessary?  Just use the first Generic parameter
        public string MixinTypeParameterReference { get; set; }
    }

    public interface IGenericMixin<in T>
    {
        T DecoratedInstance { set; }
    }

    public class GenericConstraint
    {
        public virtual string SomeMethod()
        {
            return "GenericConstraint";
        }
    }

    public class ChildGenericConstraint : GenericConstraint
    {
        public override string SomeMethod()
        {
            return "Child_" + base.SomeMethod();
        }

        public string ExtraMethod()
        {
            return "Extra Method";
        }
    }

    [GenericMixin(MixinTypeParameterReference = "T")]
    public partial class GenericDecorator<T> : IGenericMixin<T> // : T - What I want to do
        where T : GenericConstraint, new()
    {
        //private T decoratedInstance;

        //public GenericDecorator(T instance)
        //{
        //    decoratedInstance = instance;
        //}
        //public GenericDecorator()
        //{
        //    decoratedInstance = new T();
        //}

        public virtual string SomeMethod()
        {
            return "Decorator_" + DecoratedInstance.SomeMethod();
        }

        public T DecoratedInstance { set; private get; }
    }
    
    [BasicMixin(Target = typeof(GenericDecorator<ChildGenericConstraint>))]
    public partial class GenericDecoratorSpec{}

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public partial class GenericDecoratorSpec 
    {
        private sealed class __Mixins //put all auto-generated objects as child types
        {
            public __Mixins(GenericDecoratorSpec target)
            {
                //Could be reused by multiple decorators.
                _childGenericConstraintMixin =
                    new DefaultMixinActivator().CreateInstance<ChildGenericConstraint>();

                _GenericDecoratorMixin = 
                    new DefaultMixinActivator().CreateInstance<GenericDecorator<ChildGenericConstraint>>();

                _GenericDecoratorMixin.DecoratedInstance = _childGenericConstraintMixin;
            }

            public readonly ChildGenericConstraint _childGenericConstraintMixin;
            public readonly GenericDecorator<ChildGenericConstraint> _GenericDecoratorMixin;
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

        //requries method resolution to figure out this
        //should call the decorator.
        public virtual string SomeMethod()
        {
            return __mixins._GenericDecoratorMixin.SomeMethod();
        }

        public string ExtraMethod()
        {
            return __mixins._childGenericConstraintMixin.ExtraMethod();
        }

        public static implicit operator GenericDecorator<ChildGenericConstraint>(GenericDecoratorSpec target)
        {
            return target.__mixins._GenericDecoratorMixin;
        }

        public static implicit operator ChildGenericConstraint(GenericDecoratorSpec target)
        {
            return target.__mixins._childGenericConstraintMixin;
        }
    }
}