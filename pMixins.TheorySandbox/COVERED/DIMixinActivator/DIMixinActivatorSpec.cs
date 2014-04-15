//----------------------------------------------------------------------- 
// <copyright file="DIMixinActivatorSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, October 14, 2013 2:57:52 PM</date> 
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
using Ninject;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.DIMixinActivator
{
    /// <summary>
    /// This is covered in :
    ///     <see cref="MixinIsCreatedWithDependencyInjection"/>, albeit
    ///     the implementation has evolved significantly.
    /// 
    /// 
    /// <see cref="IMixinActivator"/> needs to be updated
    /// so it can manipulate the AST
    /// </summary>
    /// <remarks>
    /// Requeries <see cref="StandardKernel"/> be given 
    /// an <see cref="NinjectSettings"/> where
    /// <see cref="NinjectSettings.InjectNonPublic"/> is <c>true</c>.
    /// http://stackoverflow.com/questions/4293021/ninject-stopped-injecting-my-properties
    /// </remarks>
    public class DIPropertyInjectionActivator : IMixinActivator
    {
        public static StandardKernel kernel;

        public T CreateInstance<T>(params object[] constructorArgs)
        {
            return kernel.Get<T>();
        }
    }

    public interface IRandomDependency
    {
        string PrettyPrintName(string name);
    }

    public class DIMixinActivatorMixin
    {
        public IRandomDependency RandomDependency { get; set; }

        public DIMixinActivatorMixin(IRandomDependency randomDependency)
        {
            if (null == randomDependency)
                throw new ArgumentNullException("randomDependency");

            RandomDependency = randomDependency;
        }

        public string PrettyPrintName(string name)
        {
            return RandomDependency.PrettyPrintName(name);
        }
    }

    [BasicMixin(Target = typeof(DIMixinActivatorMixin),
        Activator = typeof(DIPropertyInjectionActivator))]
    public partial class DIMixinActivatorSpec
    {
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    public abstract class ProtectedMembersWrapper : DIMixinActivatorMixin
    {
        public ProtectedMembersWrapper(IRandomDependency randomDependency) : base(randomDependency)
        {
        }
    }

    public sealed class AbstractMembersWrapper : ProtectedMembersWrapper
    {
        public AbstractMembersWrapper(IRandomDependency randomDependency) : base(randomDependency)
        {
        }
    }

    public sealed class DIMixinActivatorMixinMasterWrapper
    {
        //Type can be Mixin directly since Protected and Abstract Wrappers aren't necessary
        public readonly DIMixinActivatorMixin Mixin;

        public DIMixinActivatorMixinMasterWrapper()
        {
            Mixin =
                (DIMixinActivatorMixin) //Explict cast in case CreateInstance returns object
                    new DIPropertyInjectionActivator().CreateInstance<DIMixinActivatorMixin>();
        }

        public string PrettyPrintName(string name)
        {
            return Mixin.PrettyPrintName(name);
        }
    }

    public partial class DIMixinActivatorSpec
    {
        private sealed class __Mixins
        {
            public static readonly global::System.Object ____Lock = new global::System.Object();

            public readonly DIMixinActivatorMixinMasterWrapper DIMixinActivatorMixinMasterWrapper;

            public __Mixins(DIMixinActivatorSpec target)
            {
                DIMixinActivatorMixinMasterWrapper = new DefaultMixinActivator().CreateInstance<
                    DIMixinActivatorMixinMasterWrapper>();
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


        public string PrettyPrintName(string name)
        {
            return ___mixins.DIMixinActivatorMixinMasterWrapper.PrettyPrintName(name);
        }
    }

/* Early version
namespace CopaceticSoftware.pMixins.TheorySandbox.DIMixinActivator
{
    public partial class DIMixinActivatorSpec
    {
        public DIMixinActivatorSpec(DIMixinActivatorMixin mixin)
        {
           // ___mixins = new __Mixins {_DIMixin = mixin};
        }

        private sealed class __Mixins //put all auto-generated objects as child types
        {
            [Inject]
            public DIMixinActivatorMixin _DIMixin { get; set; }
        }

        /// <summary>
        /// Requeries <see cref="StandardKernel"/> be given 
        /// an <see cref="NinjectSettings"/> where
        /// <see cref="NinjectSettings.InjectNonPublic"/> is <c>true</c>.
        /// http://stackoverflow.com/questions/4293021/ninject-stopped-injecting-my-properties
        /// </summary>
        /// <remarks>
        /// Must be a property, Ninject does not seem to inject
        /// fields correctly.
        /// </remarks>
        [Inject] 
        private DIMixinActivatorSpec.__Mixins __mixins { get; set; }

        
        public string PrettyPrintName(string name)
        {
            return __mixins._DIMixin.PrettyPrintName(name);
        }
    }
}
*/
}