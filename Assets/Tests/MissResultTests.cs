using Assets.Scripts.Core;
using NUnit.Framework;

namespace Assets.Tests
{
    public class MissResultTests
    {
        [Test]
        public void MissResults_InitializationWorks()
        {
            var missResult = new MissResult(5, 10);

            Assert.AreEqual(missResult.XPos, 5);
            Assert.AreEqual(missResult.YPos, 10);
        }
    }
}
