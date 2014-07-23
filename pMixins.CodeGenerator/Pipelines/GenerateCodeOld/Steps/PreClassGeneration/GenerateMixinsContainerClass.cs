//----------------------------------------------------------------------- 
// <copyright file="GeneratreMixinContainerClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, January 28, 2014 12:42:22 AM</date> 
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

using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.MixinWrappersGenerator;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PostClassGeneration;
using ICSharpCode.NRefactory.CSharp;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.PreClassGeneration
{
    /// <summary>
    /// Creates:
    /// <code><![CDATA[
    /// private sealed class __Mixins //put all auto-generated objects as child types
    ///    {
    ///         public static object ____Lock = new object();
    /// 
    ///        //Constructor created in other step
    ///         
    ///     
    ///
    ///        public readonly Lazy<MixinWithVirtualMemberWrapper> _ExampleMixin;
    ///    }
    ///
    ///    private HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins ___mixins;
    ///
    ///    private HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins __mixins
    ///    {
    ///        get
    ///        {
    ///            if (null == ___mixins)
    ///             {
    ///                 lock(__Mixins.____Lock)
    ///                 {
    ///                     if (null == ___mixins)
    ///                     {
    ///                         ___mixins = new HostCanOverrideAndExposeVirtualMixinMembersSpec.__Mixins(this);
    ///                         ___mixins.__ActivateMixinDependencies(this);
    ///                     }
    ///                 }
    ///             }
    ///
    ///            return ___mixins;
    ///        }
    ///    }
    ///
    /// ]]></code>
    /// </summary>
    /// <remarks>
    /// Constructor is created in <see cref="GenerateMixinsContainerClassConstructor"/>.
    /// </remarks>
    /// <remarks>
    /// ActivateMixinDependencies method is created in <see cref="GenerateMixinsContainerActivateMixinDependenciesMethod"/>.
    /// </remarks>
    public class GenerateMixinsContainerClass : IPipelineStep<pMixinGeneratorPipelineState>
    {
        private const string MixinContainerClassName = "__Mixins";
        public const string MixinContainerPropertyName = "___mixins";
        private const string MixinContainerPropertyBackingFieldName = "__" + MixinContainerPropertyName;

        private const string LockVariableName = "____Lock";
        

        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            var mixinContainerClassDeclaration = new TypeDeclaration
                                                     {
                                                         ClassType = ClassType.Class,
                                                         Modifiers = Modifiers.Private | Modifiers.Sealed,
                                                         Name = MixinContainerClassName,
                                                     };

            manager.MixinContainerClassGeneratorProxy = 
                manager.GeneratedClass.AddNestedType(mixinContainerClassDeclaration);
            
            CreateContainerPropertyLock(manager, LockVariableName);
            
            manager.GeneratedClass.CreateDataMember(
                "private", 
                MixinContainerClassName,
                MixinContainerPropertyBackingFieldName);

            manager.GeneratedClass.CreateProperty(
                    "private",
                    MixinContainerClassName,
                    MixinContainerPropertyName,
                    string.Format(
                        @"
                            get 
                            {{
                                if (null == {0})
                                {{
                                    lock({1}.{2})
                                    {{
                                            if (null == {0})
                                            {{
                                                {0} = new {1}(this);
                                                {0}.{3}(this);
                                            }}
                                    }}
                                }}
    
                                return {0};
                            }}
                        ",
                         MixinContainerPropertyBackingFieldName,
                         MixinContainerClassName,
                         LockVariableName,
                         GenerateMixinMasterWrapperClass.ActivateMixinDependenciesMethodName)
                    ,
                     "" //no setter
                );

            
            return true;
        }

        private void CreateContainerPropertyLock(
            pMixinGeneratorPipelineState manager,
            string lockName)
        {
            manager.MixinContainerClassGeneratorProxy
               .CreateDataMember(
                   "public static",
                   "global::System.Object",
                   lockName,
                   "= new global::System.Object();");
            
        }
    }
}
