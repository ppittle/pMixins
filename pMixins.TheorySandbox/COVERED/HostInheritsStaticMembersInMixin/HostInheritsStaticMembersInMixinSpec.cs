//----------------------------------------------------------------------- 
// <copyright file="HostInheritsStaticMembersInMixinSpec.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Monday, October 14, 2013 12:26:16 AM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.InheritanceTests;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.HostInheritsStaticMembersInMixin
{
    /// <summary>
    /// Covered in:
    ///     <see cref="MixinStaticMembersAreInjectedIntoTarget"/>
    /// </summary>
    public static class HostInheritsPublicStaticMembersInMixinMixin
    {
        public static string PublicPrettyPrint(string name)
        {
            return "Public_" + name;
        }
    }

    public class HostInheritsProtectedStaticMembersInMixinMixin
    {
        protected static string ProtectedPrettyPrint(string name)
        {
            return "Protected_" + name;
        }
    }

    [BasicMixin(Target = typeof(HostInheritsPublicStaticMembersInMixinMixin))]
    public partial class HostInheritsStaticMembersInMixinSpec
    {
        //Expose the Protected static method for testing
        public static string WrapperForBaseProtectedPrettyPrint(string name)
        {
            return ProtectedPrettyPrint(name);
        }
    }

/*/////////////////////////////////////////
/// Generated Code
/////////////////////////////////////////*/

    /// <summary>
    /// Only create wrapper for Non-Public class that has a protected static method!
    /// </summary>
    public class HostInheritsProtectedStaticMembersInMixinMixinWrapper : HostInheritsProtectedStaticMembersInMixinMixin
    {
        public static string ProtectedPrettyPrint(string name)
        {
            return HostInheritsProtectedStaticMembersInMixinMixin.ProtectedPrettyPrint(name);
        }
    }

    public partial class HostInheritsStaticMembersInMixinSpec
    {
        public static string PublicPrettyPrint(string name)
        {
            // For public static methods, can call the mixin class directly.
            return HostInheritsPublicStaticMembersInMixinMixin.PublicPrettyPrint(name);
        }

        protected static string ProtectedPrettyPrint(string name)
        {
            //For protected methods, call the wrapper
            return HostInheritsProtectedStaticMembersInMixinMixinWrapper.ProtectedPrettyPrint(name);
        }
    }
}