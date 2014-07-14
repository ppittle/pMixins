//----------------------------------------------------------------------- 
// <copyright file="MixinWithArrays.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, July 11, 2014 3:44:16 PM</date> 
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
using CopaceticSoftware.pMixins.Tests.Common.Extensions;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.IntegrationTests.CompileTests.BasicTests
{
    [TestFixture]
    public class MixinWithArrays : GenerateCodeAndCompileTestBase
    {
        long[,] a = new long[1,4];

        protected override string SourceCode
        {
            get
            { 
                return @"
                namespace Test
                {
                
                    public class MixinWithArrays
                    {
                        public int[] MethodWithArrays(int[] numbers)
                        {
                            return numbers;
                        }

                        public double[][] GetJaggedArray()
                        {
                            return new double[1][];
                        }

                        public long[,] GetMultiDimensionalArray()
                        {
                            return new long[1,4];
                        }
                    }

                    [CopaceticSoftware.pMixins.Attributes.pMixin(
                        Mixin = typeof (Test.MixinWithArrays))]
                    public partial class Target{}  
                } 
            "; }
        }

        [Test]
        public void CanExecuteMethodWithArrays()
        {
            var referenceArray = new int[] {1, 3, 4};

            var returnedArray = 
                CompilerResults.ExecuteMethod<int[]>(
                    "Test.Target",
                    "MethodWithArrays",
                    ReflectionHelper.DefaultBindingFlags,
                    referenceArray);

            referenceArray.Length.ShouldEqual(returnedArray.Length);

            for (int i = 0; i < referenceArray.Length; i++)
                referenceArray[i].ShouldEqual(returnedArray[i]);
        }

        [Test]
        public void CanGetJaggedArray()
        {
            CompilerResults.ExecuteMethod<double[][]>(
                "Test.Target",
                "GetJaggedArray")
                .ShouldNotBeNull();
        }

        [Test]
        public void CanGetMultiDimensionalArray()
        {
            CompilerResults.ExecuteMethod<long[,]>(
                "Test.Target",
                "GetMultiDimensionalArray")
                .ShouldNotBeNull();
        }
    }
}
