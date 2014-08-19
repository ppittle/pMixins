//----------------------------------------------------------------------- 
// <copyright file="MemberCollectorTests.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Tuesday, August 19, 2014 10:28:26 AM</date> 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CopaceticSoftware.CodeGenerator.StarterKit.Extensions;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.CreateCodeGenerationPlan.Steps;
using CopaceticSoftware.pMixins.CodeGenerator.Pipelines.ResolveAttributes.Infrastructure;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.CodeGenerator.Tests.UnitTests
{
    /// <summary>
    /// Tests for <see cref="CollectAllMembers.MemberCollector"/>
    /// </summary>
    [TestFixture]
    public class MemberCollectorTests
    {
        private readonly ICompilation _compilation;

        private readonly CollectAllMembers.MemberCollector _memberCollector;

        public MemberCollectorTests()
        {
            _memberCollector = new CollectAllMembers.MemberCollector();

            #region Initialize _compilation
            var referencedAssemblies = new Type[]
            {
                typeof (System.Exception),
                typeof (ICollection),
                typeof (IList<>)
            }
            .Select(t => t.Assembly.Location);
            
            IProjectContent dummyProject = new CSharpProjectContent();

            dummyProject =
                dummyProject.AddAssemblyReferences(
                    referencedAssemblies
                        .Distinct()
                        .Select(
                            a => new CecilLoader().LoadAssemblyFile(a)));

            _compilation = 
                new DefaultSolutionSnapshot(new []{dummyProject})
                    .GetCompilation(dummyProject);
            #endregion
        }
        
        [Test]
        public void CollectMembersForList()
        {
            var listType = typeof (List<int>);

            var reflectionMembers = 
                listType.GetMembers(
                    BindingFlags.Instance |
                    BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Static 
                    | BindingFlags.GetProperty | BindingFlags.SetProperty);

            var collectedMembers =
                _memberCollector.CollectMemberWrappers(
                    new pMixinAttributeResolvedResult(null)
                    {
                        Mixin = listType.ToIType(_compilation),
                    },
                    _compilation);


            //The correct number of members for List 'of' int is 78
            //because ... reasons.s
            collectedMembers.Count().ShouldEqual(78);

            /*  Extra methods that were helpful in debugging.
            var rawMembers =
                listType.ToIType(_compilation)
                    .GetMembers()
                    .Union(
                        listType.ToIType(_compilation)
                            .GetAllBaseTypes()
                            .Where(t => t.GetDefinition().Kind == TypeKind.Interface)
                            .SelectMany(t => t.GetMembers()))
                    .DistinctMembers();

            var missingMembers =
                reflectionMembers
                    .Where(m =>
                        !collectedMembers.Select(x => x.Member.Name)
                            .Contains(m.Name));


            var interfaceMembers =
                 listType.ToIType(_compilation)
                    .GetAllBaseTypes()
                    .Where(t => t.GetDefinition().Kind == TypeKind.Interface)
                    .SelectMany(t =>
                        t.GetMembers());


            var isReadOnly =
                interfaceMembers
                    .Where(x => x.Name == "IsReadOnly")
                    .ToList();

            var debug = isReadOnly.Select(x => x.GetMemberSignature())
                .ToList();

            */
        }
    }
}
