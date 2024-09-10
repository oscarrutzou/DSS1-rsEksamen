using ShamansDungeon.Other;
using Microsoft.Xna.Framework;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RotateTest()
        {
            Vector2 startPos = new(0, 1);
            float rotation = -MathHelper.PiOver2;

            Vector2 expectedPos = new(1, 0);
            Vector2 resultPos = BaseMath.Rotate(startPos, rotation);
            Assert.IsTrue(Vector2.Distance(expectedPos, resultPos) < 1e-5);
        }
    }
}