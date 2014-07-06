using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace CopaceticSoftware.pMixins.TheorySandbox.MulticastRule
{
    public class MulticastRuleSpecTest : SpecTestBase
    {
        private MulticastRuleSpec _spec;

        protected override void Establish_context()
        {
            _spec = new MulticastRuleSpec();
        }

        [Test]
        public void CanCallMulticastMixinMethod()
        {
            _spec.GetNumber().ShouldEqual(42);
        }
    }
}