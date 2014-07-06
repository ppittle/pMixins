using System;
using System.Collections.Generic;

namespace CopaceticSoftware.pMixins.TheorySandbox
{
    /// <summary>
    /// Basis for applying mulitcast rules.  Inheritors can be located anywhere,
    /// pMixin code generator will find them!
    /// </summary>
    public interface IMixinMulticastRule
    {
        IEnumerable<Type> MixinsForTypes(string typeFullName);
    }
}