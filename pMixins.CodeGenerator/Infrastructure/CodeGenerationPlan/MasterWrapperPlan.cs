﻿//----------------------------------------------------------------------- 
// <copyright file="MasterWrapperPlan.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Saturday, July 26, 2014 12:21:54 PM</date> 
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

using System.Collections.Generic;
using CopaceticSoftware.Common.Extensions;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Infrastructure.CodeGenerationPlan
{
    public class MasterWrapperPlan
    {
        /// <summary>
        /// The variable name for the data member that holds a reference
        /// to the Mixin (with a type of IRequirements interface).
        /// </summary>
        public const string MixinInstanceDataMemberName = "_mixinInstance";

        /// <summary>
        /// Parent Mixin Generation Plan
        /// </summary>
        public MixinGenerationPlan MixinGenerationPlan { get; set; }

        /// <summary>
        /// The Constructor paramater name for the Mixin reference
        /// (with a type of IRequirements interface).  This 
        /// parameter is assigned to <see cref="MixinInstanceDataMemberName"/>
        /// (via <see cref="MixinInstanceInitializationStatement"/>)
        /// and is needed to generate <see cref="MixinInstanceInitializationStatement"/>.
        /// </summary>
        public const string TargetInstanceConstructorParameterName = "target";

        public string MasterWrapperClassName { get; set; }

        /// <summary>
        /// The variable name of the Master Wrapper inside the Mixins container
        /// <code>
        /// <![CDATA[
        /// private sealed class __Mixins
		/// { 	
        ///     //MasterWrapperInstanceNameInMixinsContainer = Test_ExampleMixin
		/// 	public readonly __pMixinAutoGenerated.Test_ExampleMixin.ExampleMixinMasterWrapper Test_ExampleMixin;
		/// }
        /// ]]></code>
        /// </summary>
        public string MasterWrapperInstanceNameInMixinsContainer { get; set; }

        /// <summary>
        /// The Master Wrapper instance variable name scoped to the level
        /// of the <see cref="TargetCodeBehindPlan"/>.
        /// <code>
        /// <![CDATA[
        /// public partial class Target
        /// {
        ///     public string MixinMethod()
        ///     {    
        ///         //MasterWrapperInstanceNameAvailableFromTargetCodeBehind = ___mixins.Test_MixinWithAttributes
        ///         return ___mixins.Test_MixinWithAttributes.MixinMethod();
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </summary>
        public string MasterWrapperInstanceNameAvailableFromTargetCodeBehind
        {
            get
            {
                return
                    MixinGenerationPlan.CodeGenerationPlan.TargetCodeBehindPlan.MixinsPropertyName
                        .EnsureEndsWith(".") +
                    MasterWrapperInstanceNameInMixinsContainer;
            }
        }

        /// <summary>
        /// The Full Type Name for <see cref="MixinInstanceDataMemberName"/>.
        /// </summary>
        public string MixinInstanceTypeFullName { get; set; }

        /// <summary>
        /// Statement for initializing the 
        /// <see cref="MixinInstanceDataMemberName"/>
        /// <code>
        /// <![CDATA[
        /// base.TryActivateMixin<__pMixinAutoGenerated.Test_ExampleMixin.ExampleMixinAbstractWrapper> (target)
        /// ]]></code>
        /// </summary>
        public string MixinInstanceInitializationStatement { get; set; }

        public IEnumerable<IType> MixinDependencies { get; set; }

        #region Memebers

        public IEnumerable<MemberWrapper> StaticMembers { get; set; }

        public IEnumerable<MemberWrapper> ProtectedAbstractMembers { get; set; }

        public IEnumerable<MemberWrapper> VirtualMembers { get; set; }

        public IEnumerable<MemberWrapper> RegularMembers { get; set; }

        #endregion
    }
}
