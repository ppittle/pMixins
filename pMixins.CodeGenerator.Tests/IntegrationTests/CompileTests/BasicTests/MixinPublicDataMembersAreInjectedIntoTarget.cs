//----------------------------------------------------------------------- 
// <copyright file="MixinPublicDataMembersAreInjectedIntoTarget.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Wednesday, January 29, 2014 10:57:24 PM</date> 
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

using System.Reflection;
using CopaceticSoftware.pMixins.CodeGenerator.Tests.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinPublicDataMembersAreInjectedIntoTarget : GenerateCodeAndCompileTestBase
    {
        protected override string SourceCode
        {
            get
            {
                return
                    @"
                        namespace Test
                        {
                            public class MixinWithPublicDataMembers
                            {
                                public string DataMemberForReadTest = ""ReadTest"";      

                                public string DataMemberForReadWriteTest;                          
                                
                                public int IntegerDataMember = 1;
                                public System.Int32 IntegerDataMember2 = 42;
                                public float FloatDataMember = 2f;
                                public short ShortDataMember = 3;
                                public double DoubleDataMember = 4d;
                                public long LongDataMember = 5L;
                                public char CharDataMember = '6';
                                public bool BoolDataMember = true;
                                public byte ByteDataMember = 0;
                                public uint UIntDataMember = 8;
                            }

                            [CopaceticSoftware.pMixins.Attributes.pMixin(Mixin = typeof (Test.MixinWithPublicDataMembers))]
                            public partial class Target{}                        
                        }
                    ";
            }
        }

        [Test]
        public void CanReadDataMember()
        {
            //Assume DataMember will be exposed as Property

            CompilerResults
                .ExecutePropertyGet<string>(
                    "Test.Target",
                    "DataMemberForReadTest",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public) 
                .ShouldEqual("ReadTest");

            CompilerResults
                .ExecutePropertyGet<int>(
                    "Test.Target",
                    "IntegerDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public) 
                .ShouldEqual(1);

            CompilerResults
                .ExecutePropertyGet<int>(
                    "Test.Target",
                    "IntegerDataMember2",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual(42);

            CompilerResults
                .ExecutePropertyGet<float>(
                    "Test.Target",
                    "FloatDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public) 
                .ShouldEqual(2f);

            CompilerResults
                .ExecutePropertyGet<short>(
                    "Test.Target",
                    "ShortDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual((short)3);

            CompilerResults
                .ExecutePropertyGet<double>(
                    "Test.Target",
                    "DoubleDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual(4d);

            CompilerResults
                .ExecutePropertyGet<long>(
                    "Test.Target",
                    "LongDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual(5L);

            CompilerResults
                .ExecutePropertyGet<char>(
                    "Test.Target",
                    "CharDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual('6');

            CompilerResults
                .ExecutePropertyGet<bool>(
                    "Test.Target",
                    "BoolDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual(true);

            CompilerResults
                .ExecutePropertyGet<byte>(
                    "Test.Target",
                    "ByteDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual(byte.Parse("0"));

            CompilerResults
                .ExecutePropertyGet<uint>(
                    "Test.Target",
                    "UIntDataMember",
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .ShouldEqual((uint)8);
        }

        [Test]
        public void CanWriteToDataMember()
        {
            //Assume DataMember will be exposed as Property

            const string getSetDataMemberTest = "SuperDataMEmberTest!!";

            var targetInstance = CompilerResults.TryLoadCompiledType("Test.Target");

            if (null == targetInstance)
                Assert.Fail("Failed to load Test.Target instance");

            ReflectionHelper.ExecutePropertySet(
                targetInstance,
                "DataMemberForReadWriteTest",
                getSetDataMemberTest);

            ReflectionHelper.ExecutePropertyGet<string>(
                targetInstance,
                "DataMemberForReadWriteTest")
                .ShouldEqual(getSetDataMemberTest);
        }
    }
}
