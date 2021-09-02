using Assets.Scripts.Core;
using NUnit.Framework;

namespace Assets.Tests
{
    public class SquareTests
    {
        [Test]
        public void SquareInitialization_DefaultsToVacantState()
        {
            var square = new Square();

            Assert.AreEqual(square.State, State.Vacant);
        }
    }
}
