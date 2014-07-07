using NUnit.Framework;

namespace pMixins.Mvc.Recipes.VirtualMethodOverride
{
    public class VirtualMethodOverrideTest
    {
        [Test]
        public void MemberOverriddenInTarget()
        {
            Assert.AreEqual(
                "Target",
                new VirtualMethodOverride().GetName());
        }

        [Test]
        public void MemberOverriddenInChild()
        {
            Assert.AreEqual(
                "Child",
                new Child().GetName());
        }
    }
}