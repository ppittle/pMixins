using System;
using System.Collections.Generic;

namespace CopaceticSoftware.pMixins.TheorySandbox.MulticastRule
{
    public class MulticastMixin
    {
        public int GetNumber()
        {
            return 42;
        }
    }

    public class MulticastMixinRule : IMixinMulticastRule
    {
        public IEnumerable<Type> MixinsForTypes(string typeFullName)
        {
            yield return typeof (MulticastMixin);
        }
    }
}