//----------,------------------------------------------------------------- 
// <copyright file="AsIsWraperSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, August 23, 2013 1:35:47 AM</date> 
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
using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.ConversionTests;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.AsIsWrapper
{
    
    #region As/Is Helper
    /// <summary>
    /// Covered by:
    ///  <see cref="AsIsHelperTest"/>   
    /// 
    /// 
    /// Mixins will not natively support as/is operators 
    /// http://stackoverflow.com/questions/18390664/c-sharp-implicit-conversion-operator-and-is-as-operator,
    /// but this helper class, well helps
    /// </summary>
    public static class AsIsHelper
    {
        public static T As<T>(this object obj) 
            where T : class
        {
            var objAsIWrap = obj as IWrap<T>;

            return (null != objAsIWrap)
                ? objAsIWrap.GetWrappedItem()
                : obj as T;
        }

        public static bool Is<T>(this object obj)
        {
            var objAsIWrap = obj as IWrap<T>;

            return (null != objAsIWrap || obj is T);
        }
    }
    #endregion

    #region As/Is Interface
    public interface IWrap<out T>
    {
        T GetWrappedItem();
    }
    #endregion

    #region Spec

    public class ExampleMixin
    {
        public string Foo()
        {
            return "Foo";
        }
    }

    [BasicMixin(Target = typeof(ExampleMixin))]
    public partial class AsIsWrapperSpec { }

/*/////////////////////////////////////////
    /// Generated Code
    /////////////////////////////////////////*/

    public partial class AsIsWrapperSpec : IWrap<ExampleMixin>
    {
        private class __Mixins
        {
            public readonly Lazy<ExampleMixin> _ExampleMixin = new Lazy<ExampleMixin>(
                () => new DefaultMixinActivator().CreateInstance<ExampleMixin>());
        }

        private readonly __Mixins __mixins = new __Mixins();

        public string Foo()
        {
            return __mixins._ExampleMixin.Value.Foo();
        }

        public static implicit operator ExampleMixin(AsIsWrapperSpec spec)
        {
            return spec.__mixins._ExampleMixin.Value;
        }

        ExampleMixin IWrap<ExampleMixin>.GetWrappedItem()
        {
            return this.__mixins._ExampleMixin.Value;
        }
    }

    #endregion
}