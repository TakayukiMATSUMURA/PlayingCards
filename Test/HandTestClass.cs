using System.Linq;
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
            Assert.AreEqual(Poker.HandRank.HighCard, hand.Rank);
        }

        [Test()]
        public void TestOnePair()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.T, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(4, Suit.Spade),
                new Poker.Card(Card.T, Suit.Club),
                new Poker.Card(2, Suit.Heart)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.OnePair, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(5);
            cards.Sort();
            cards.Reverse();
            var usedCards = cards.Where(x => x.Rank == Card.T).ToList();
            usedCards.AddRange(cards.Where(x => !usedCards.Contains(x)).Take(3));
            
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestTwoPair()
        {
            var cards = new List<Card>
            {
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(Card.K, Suit.Diamond),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(2, Suit.Heart),
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(Card.K, Suit.Club),
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.TwoPair, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(3);
            cards.Sort();
            cards.Reverse();
            var usedCards = cards.Where(x => x.Rank == Card.K).ToList();
            usedCards.AddRange(cards.Where(x => x.Rank == 6));
            usedCards.Add(cards[0]);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestThreeOfAKind()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(2, Suit.Heart),
                new Poker.Card(3, Suit.Spade)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.ThreeOfAKind, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(4);
            cards.Sort();
            cards.Reverse();
            var aces = cards.Where(x => x.Rank == Poker.Card.A).ToList();
            var usedCards = aces;
            usedCards.AddRange(cards.Where(x => !usedCards.Contains(x)).Take(2));
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestStraight()
        {
            var cards = new List<Card>
            {
                new Poker.Card(5, Suit.Spade),
                new Poker.Card(8, Suit.Diamond),
                new Poker.Card(6, Suit.Club),
                new Poker.Card(6, Suit.Heart),
                new Poker.Card(4, Suit.Heart),
                new Poker.Card(7, Suit.Club)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.Straight, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(2);
            var usedCards = cards;
            usedCards.Sort();
            usedCards.Reverse();
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestWheel()
        {
            var cards = new List<Card>
            {
                new Poker.Card(4, Suit.Heart),
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(5, Suit.Club),
                new Poker.Card(Card.T, Suit.Club),
                new Poker.Card(2, Suit.Heart),
                new Poker.Card(3, Suit.Spade)
            };
            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.Straight, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(3);
            var usedCards = cards;
            usedCards.Sort();
            usedCards.Reverse();
            usedCards.Add(usedCards[0]);
            usedCards.RemoveAt(0);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestFlush()
        {
            var cards = new List<Card>
            {
                new Poker.Card(6, Suit.Diamond),
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.K, Suit.Diamond),
                new Poker.Card(6, Suit.Heart),
                new Poker.Card(4, Suit.Diamond),
                new Poker.Card(2, Suit.Diamond)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.Flush, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Suit == Suit.Diamond).ToList();
            usedCards.Sort();
            usedCards.Reverse();
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestFullHouse()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(6, Suit.Heart),
                new Poker.Card(Card.A, Suit.Club),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(8, Suit.Heart),
                new Poker.Card(6, Suit.Club)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.FullHouse, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Rank == 6).ToList();
            usedCards.Sort();
            usedCards.Reverse();
            var aces = cards.Where(x => x.Rank == Poker.Card.A).ToList();
            aces.Sort();
            aces.Reverse();
            usedCards.AddRange(aces);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestFourOfAKind()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Spade),
                new Poker.Card(Card.T, Suit.Club),
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(6, Suit.Club),
                new Poker.Card(Card.A, Suit.Club),
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.FourOfAKind, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Rank == Poker.Card.A).ToList();
            usedCards.Sort();
            usedCards.Reverse();
            usedCards.Add(cards[1]);
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public void TestStraightFlush()
        {
            var cards = new List<Card>
            {
                new Poker.Card(Card.J, Suit.Spade),
                new Poker.Card(Card.T, Suit.Spade),
                new Poker.Card(8, Suit.Spade),
                new Poker.Card(9, Suit.Diamond),
                new Poker.Card(Card.Q, Suit.Spade),
                new Poker.Card(9, Suit.Spade)
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.StraightFlush, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Suit == Suit.Spade).ToList();
            usedCards.Sort();
            usedCards.Reverse();
            for (var i = 0; i < 5; i++)
            {
                //Assert.AreEqual(usedCards[i], hand.Cards[i]);
                Assert.AreEqual(usedCards[i].Rank, hand.Cards[i].Rank);
                Assert.AreEqual(usedCards[i].Suit, hand.Cards[i].Suit);
            }
        }
    }
}
