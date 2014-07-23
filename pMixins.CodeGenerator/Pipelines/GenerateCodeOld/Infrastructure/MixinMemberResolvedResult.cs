//----------------------------------------------------------------------- 
// <copyright file="MixinMember.cs" company="Copacetic Software"> 
// Copyright (c) Copacetic Software.  
// <author>Philip Pittle</author> 
// <date>Friday, February 7, 2014 8:50:16 PM</date> 
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

using ICSharpCode.NRefactory.TypeSystem;
using JetBrains.Annotations;

namespace CopaceticSoftware.pMixins.CodeGenerator.Pipelines.GenerateCode.Infrastructure
{
    public class MixinMemberResolvedResult
    {
        public IMember Member { get; set; }
        [CanBeNull]
        public IType ExplicitImplementationInterfaceType { get; set; }
        public bool ImplementExplicitly { get { return null != ExplicitImplementationInterfaceType; } }
    }
}
