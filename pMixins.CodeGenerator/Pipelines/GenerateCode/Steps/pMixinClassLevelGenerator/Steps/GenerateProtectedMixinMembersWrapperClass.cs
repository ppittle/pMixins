//----------------------------------------------------------------------- 
// <copyright file="GenerateProtectedMixinMembersWrapperClass.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Sunday, February 2, 2014 7:10:32 PM</date> 
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
using System.Linq;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.Common.Patterns;
using CopaceticSoftware.pMixins.CodeGenerator.Infrastructure;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Steps.pMixinClassLevelGenerator.Steps
{
    /// <summary>
    /// Generates a class (mixin.Name + ProtectedMemberWrapper) that promotes
    /// all Mixin's protected members and constructors to public
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// public abstract class ProtectedMembersWrapper : 
    ///        global::CopaceticSoftware.pMixins.TheorySandbox.MixinIsAbstractWithProtectedNonParameterlessConstructor.
    ///        AbstractWithProtectedNonParameterlessConstructorMixin
    ///    {
    ///        public ProtectedMembersWrapper(string s) : base(s)
    ///        {
    ///        }
    ///
    ///        // "New" keyword is required
    ///        public new int ProtectedMethod()
    ///        {
    ///            return base.ProtectedMethod();
    ///        }
    ///
    ///        public override string ProtectedAbstractMethod()
    ///        {
    ///            //Can throw not implemented because this method will never be called.
    ///            //it will be overwritten in the abstract wrapper
    ///            throw new System.NotImplementedException();
    ///        }
    ///
    ///        public new virtual string ProtectedVirtualMethod(int i)
    ///        {
    ///            return base.ProtectedVirtualMethod(i);
    ///        }
    ///
    ///        //Upgrade static members as well.  "New" keyword is required.
    ///        public new static string ProtectedStaticMethod()
    ///        {
    ///            return "protected static method";
    ///        }
    ///    }
    /// ]]>
    /// </code>
    /// </example>
    public class GenerateProtectedMixinMembersWrapperClass : IPipelineStep<pMixinGeneratorPipelineState> 
    {
        public static string GetWrapperClassName(pMixinGeneratorPipelineState manager)
        {
            return manager.CurrentpMixinAttribute.Mixin.GetNameAsIdentifier() + "ProtectedMembersWrapper";
        }


        public bool PerformTask(pMixinGeneratorPipelineState manager)
        {
            if (manager.CurrentpMixinAttribute.Mixin.IsStaticOrSealed())
                return true;
            
            var wrapperClassDeclaration = new TypeDeclaration
                                          {
                                              ClassType = ClassType.Class,
                                              Modifiers = Modifiers.Abstract | GetMixinTypeModifiers(manager),
                                              Name = GetWrapperClassName(manager)
                                          };

            var simpleType = new SimpleType(
                manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal());

            //Have class inherit from mixin
            wrapperClassDeclaration.BaseTypes.Add(simpleType);

            manager.CurrentMixinProtectedMembersWrapperClass = wrapperClassDeclaration;

            ICodeGeneratorProxy wrapperClass;

            if (manager.CurrentpMixinAttribute.Mixin.GetDefinition().IsAbstract)
            {
                //Protected/Abstract Wrappers will all be created in the external
                //namespace (Mixin is not allowed to be private)
                manager.BaseState.GeneratedCodeSyntaxTree.AddChildTypeDeclaration(
                    wrapperClassDeclaration,
                    new NamespaceDeclaration(ExternalGeneratedNamespaceHelper.GenerateNamespace(manager)));

                wrapperClass = new CodeGeneratorProxy(wrapperClassDeclaration, "");
            }
            else
            {
                //Protected/Abstract Wrappers will all be created as 
                //a nested type inside Target (in case Mixin is private)

                wrapperClass = manager.CurrentAutoGeneratedTypeDeclaration.AddNestedType(wrapperClassDeclaration);    
            }
            
            
            ProcessNonAbstractMembers(wrapperClass, manager);

            //Ignore Abstract Members - Let the AbstractWrapper promote them
            //ProcessAbstractMembers(wrapperClass, manager);

            ProcessConstructors(wrapperClass, manager);
            
            return true;
        }

        /// <summary>
        /// Also used by <see cref="GenerateAbstractMixinMembersWrapperClass"/>.
        /// TODO: find a better place for this logic.
        /// </summary>
        public static Modifiers GetMixinTypeModifiers(pMixinGeneratorPipelineState manager)
        {
            var mixinDefinition = manager.CurrentpMixinAttribute.Mixin.GetDefinition();

            if (mixinDefinition.IsInternal)
                return Modifiers.Internal;

            if (mixinDefinition.IsProtected)
                return  Modifiers.Protected;

            return Modifiers.Public;
        }

        private void ProcessNonAbstractMembers(ICodeGeneratorProxy wrapperClass, pMixinGeneratorPipelineState manager)
        {
            var proxyMemberHelper = 
                new CodeGeneratorProxyMemberHelper(wrapperClass,
                    manager.BaseState.Context.TypeResolver.Compilation);


            //Promote protected members to public
            proxyMemberHelper.CreateMembers(
                manager.CurrentMixinMembers
                    .Select(x => x.Member)
                    .Where(member => member.IsProtected &&
                        //Handle abstract members specially
                        !member.IsAbstract),
                generateMemberModifier:
                    member => "public new" + ((member.IsVirtual) ? " virtual" : "") + (member.IsStatic ? " static" : ""),
                baseObjectIdentifierFunc:
                    member => (member.IsStatic) 
                        ? manager.CurrentpMixinAttribute.Mixin.GetOriginalFullNameWithGlobal() 
                        : "base");
        }

        [Obsolete("Ignore Abstract Members - Let the AbstractWrapper promote them", true)]
        private void ProcessAbstractMembers(ICodeGeneratorProxy wrapperClass, pMixinGeneratorPipelineState manager)
        {
            //Members will be implemented in AbstractWrapperClass
            const string throwExceptionSource = "throw new global::System.NotImplementedException();";

            foreach (var aMember in manager.CurrentpMixinAttribute.Mixin.GetMembers()
                .Where(m => m.Accessibility == Accessibility.Protected &&
                            m.IsAbstract))
            {
                if (aMember is IMethod)
                {
                    wrapperClass.CreateMethod(
                        "public override",
                        aMember.ReturnType.GetOriginalFullNameWithGlobal(),
                        aMember.Name,
                        (aMember as IMethod).Parameters.ToKeyValuePair(),
                        throwExceptionSource,
                        (aMember as IMethod).GetGenericMethodConstraints(
                            manager.BaseState.Context.TypeResolver.Compilation));
                }

                else if (aMember is IProperty)
                {
                    wrapperClass.CreateProperty(
                        "public override",
                        aMember.ReturnType.GetOriginalFullNameWithGlobal(),
                        aMember.Name,
                        (aMember as IProperty).CanGet ? throwExceptionSource : "",
                        (aMember as IProperty).CanSet ? throwExceptionSource : "" );
                }
            }
        }

        private void ProcessConstructors(ICodeGeneratorProxy wrapperClass, pMixinGeneratorPipelineState manager)
        {
            //Promote protected constructors to public and copy public constructors (in case of non-parameterless constructor)
            foreach (var constructor in manager.CurrentpMixinAttribute.Mixin.GetConstructors(
                c => !c.IsPrivate))
            {
                wrapperClass.CreateConstructor(
                    "public",
                    constructor.Parameters.ToKeyValuePair(),
                    ": base(" + string.Join(",", constructor.Parameters.Select(x => x.Name)) + ")",
                    string.Empty);
            }
        }
    }
}
