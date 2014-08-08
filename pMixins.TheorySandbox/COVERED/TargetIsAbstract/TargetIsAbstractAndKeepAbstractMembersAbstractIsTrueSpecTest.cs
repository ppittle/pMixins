//----------------------------------------------------------------------- 
// <copyright file="TargetIsAbstractAndKeepAbstractMembersAbstractIsTrueSpecTest.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, August 8, 2014 10:12:36 PM</date> 
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

using CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.AdvancedMixinTypes;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.COVERED.TargetIsAbstract
{
    /// <summary>
    /// Covered in:
    ///     <see cref="TargetIsAbstractAndKeepAbstractMembersAbstractIsTrue"/>
    /// </summary>
    public class TargetIsAbstractAndKeepAbstractMembersAbstractIsTrueSpecTest : SpecTestBase
    {
        public class Child : TargetIsAbstractAndKeepAbstractMembersAbstractIsTrueSpec
        {
            public override int Number
            {
                get { return 42; }
            }
        }

        [Test]
        public void CanCallMixedInMethod()
        {
            new Child().GetMagicNumber().ShouldEqual(42);
        }
    }
}
