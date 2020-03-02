using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Card = PlayingCards.Default.Card;
using System.Threading.Tasks;

namespace PlayingCards.Test
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
                Assert.AreEqual(usedCards[i], hand.Cards[i]);
            }
        }

        [Test()]
        public async Task TestEquity()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Heart),
                    new Poker.Card(2, Suit.Diamond)
                },
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Diamond),
                    new Poker.Card(8, Suit.Diamond)
                }
            };
            var communityCards = new List<Card> { };
            var result = await Poker.Hand.EquityCalculator.Calc(hands, communityCards);
            Assert.AreEqual(32.97f, result[0].Total);
            Assert.AreEqual(22.73f, result[0].Win);
            Assert.AreEqual(20.49f, result[0].Split);
            Assert.AreEqual(67.03f, result[1].Total);
            Assert.AreEqual(56.78f, result[1].Win);
            Assert.AreEqual(20.49f, result[1].Split);
        }

        [Test()]
        public async Task TestEquityWith3Hands()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Spade),
                    new Poker.Card(Card.K, Suit.Heart),
                },
                new List<Card>
                {
                    new Poker.Card(9, Suit.Diamond),
                    new Poker.Card(8, Suit.Diamond)
                },
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Club),
                    new Poker.Card(8, Suit.Spade)
                }
            };
            var communityCards = new List<Card> { };
            var result = await Poker.Hand.EquityCalculator.Calc(hands, communityCards);
            Assert.AreEqual(55.28f, result[0].Total);
            Assert.AreEqual(53.32f, result[0].Win);
            Assert.AreEqual(4.02f, result[0].Split);
            Assert.AreEqual(31.18f, result[1].Total);
            Assert.AreEqual(30.34f, result[1].Win);
            Assert.AreEqual(1.79f, result[1].Split);
            Assert.AreEqual(13.54f, result[2].Total);
            Assert.AreEqual(10.85f, result[2].Win);
            Assert.AreEqual(5.48f, result[2].Split);
        }

        [Test()]
        public async Task TestEquityOnChop()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Spade),
                    new Poker.Card(2, Suit.Heart),
                },
                new List<Card>
                {
                    new Poker.Card(Card.A, Suit.Club),
                    new Poker.Card(2, Suit.Spade)
                }
            };
            var communityCards = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Heart),
                new Poker.Card(Card.A, Suit.Diamond),
                new Poker.Card(Card.K, Suit.Heart),
                new Poker.Card(Card.J, Suit.Spade),
                new Poker.Card(Card.T, Suit.Spade),
            };

            var result = await Poker.Hand.EquityCalculator.Calc(hands, communityCards);
            Assert.AreEqual(50f, result[0].Total);
            Assert.AreEqual(0, result[0].Win);
            Assert.AreEqual(100f, result[0].Split);
            Assert.AreEqual(50f, result[1].Total);
            Assert.AreEqual(0, result[1].Win);
            Assert.AreEqual(100f, result[1].Split);
        }

        [Test()]
        public async Task TestOuts()
        {
            // WSOP 2019 last hand
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Poker.Card(8, Suit.Spade),
                    new Poker.Card(4, Suit.Spade)
                },
                new List<Card>
                {
                    new Poker.Card(Card.K, Suit.Heart),
                    new Poker.Card(Card.K, Suit.Club)
                }
            };
            var communityCards = new List<Card>
            {
                new Poker.Card(Card.T, Suit.Spade),
                new Poker.Card(2, Suit.Diamond),
                new Poker.Card(6, Suit.Spade),
                new Poker.Card(9, Suit.Club)
            };

            var result = await Poker.Hand.EquityCalculator.Calc(hands, communityCards);
            var outs = new List<Card>
            {
                new Poker.Card(Card.A, Suit.Spade),
                new Poker.Card(Card.K, Suit.Spade),
                new Poker.Card(Card.Q, Suit.Spade),
                new Poker.Card(Card.J, Suit.Spade),
                new Poker.Card(9, Suit.Spade),
                new Poker.Card(7, Suit.Spade),
                new Poker.Card(5, Suit.Spade),
                new Poker.Card(3, Suit.Spade),
                new Poker.Card(2, Suit.Spade),
                new Poker.Card(7, Suit.Heart),
                new Poker.Card(7, Suit.Diamond),
                new Poker.Card(7, Suit.Club)
            };

            Assert.AreEqual(outs.Count, result[0].Outs.Count);
            for(var i = 0; i < outs.Count; i++)
            {
                Assert.AreEqual(outs[i], result[0].Outs[i]);
            }
        }
    }
}
