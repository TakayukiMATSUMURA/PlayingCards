using NUnit.Framework;

namespace PlayingCards.Test
{
    using Card = Default.Card;

    [TestFixture()]
    public class CardTestClass
    {
        [Test()]
        public void TestToString()
        {
            var card = new Card(Card.A, Suit.Spade);
            Assert.AreEqual("As", card.ToString());
        }

        [Test()]
        public void TestEqualOperator()
        {
            var a = new Card(Card.A, Suit.Spade);
            var b = new Card(Card.A, Suit.Spade);
            Assert.AreEqual(true, a == b);
        }

        [Test()]
        public void Testlte()
        {
            var a = new Card(Card.A, Suit.Diamond);
            var b = new Card(Card.A, Suit.Spade);
            Assert.AreEqual(true, a <= b);
        }
    }
}
