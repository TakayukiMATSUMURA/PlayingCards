using NUnit.Framework;

namespace PlayingCards.Test
{
    [TestFixture()]
    public class CardTestClass
    {
        [Test()]
        public void TestToString()
        {
            var card = new Card("As");
            Assert.AreEqual("As", card.ToString());
        }

        [Test()]
        public void TestEqualOperator()
        {
            var a = new Card("As");
            var b = new Card("As");
            Assert.AreEqual(true, a == b);
        }

        [Test()]
        public void Testlte()
        {
            var a = new Card("Ad");
            var b = new Card("As");
            Assert.AreEqual(true, a <= b);
        }
    }
}
