using System.Collections.Generic;
using NUnit.Framework;
using PlayingCards;
using Poker = PlayingCards.Poker;

namespace Test
{
    [TestFixture()]
    public class HandTestClass
    {
        [Test()]
        public void TestNoPair()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.K, Suit.Club),
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(4, Suit.Spade),
                new Poker.Card(2, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xdc531, hand.Code);
        }

        [Test()]
        public void TestOnePair()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(4, Suit.Spade),
                new Poker.Card(2, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xd53100, hand.Code);
        }

        [Test()]
        public void TestTwoPair()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(2, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xd510000, hand.Code);
        }

        [Test()]
        public void TestThreeOfAKind()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(2, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xd5100000, hand.Code);
        }

        [Test()]
        public void TestStraight()
        {
            var cards = new List<Card>
            {
                new Poker.Card(8, Suit.Diamond),
                new Poker.Card(7, Suit.Club),
                new Poker.Card(6, Suit.Heart),
                new Poker.Card(5, Suit.Spade),
                new Poker.Card(4, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0x700000000, hand.Code);
        }

        [Test()]
        public void TestFlush()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.K, Suit.Diamond),
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(4, Suit.Diamond),
                new Poker.Card(2, Suit.Diamond)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xdc53100000, hand.Code);
        }

        [Test()]
        public void TestFullHouse()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(6, Suit.Heart),
                new Poker.Card(6, Suit.Club)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0x5d000000000, hand.Code);
        }

        [Test()]
        public void TestFourOfAKind()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Spade),
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Club)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xd50000000000, hand.Code);
        }

        [Test()]
        public void TestStraightFlush()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.Q, Suit.Spade),
                new Poker.Card(Card.J, Suit.Spade),
                new Poker.Card(Card.T, Suit.Spade),
                new Poker.Card(9, Suit.Spade),
                new Poker.Card(8, Suit.Spade)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(0xb000000000000, hand.Code);
        }
    }
}
