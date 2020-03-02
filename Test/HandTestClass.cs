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
                new Poker.Card("Ad"),
                new Poker.Card("Kc"),
                new Poker.Card("6d"),
                new Poker.Card("4s"),
                new Poker.Card("2h")
            };

            var hand = new Poker.Hand(cards);
            Assert.AreEqual(Poker.HandRank.HighCard, hand.Rank);
        }

        [Test()]
        public void TestOnePair()
        {
            var cards = new List<Card>
            {
                new Poker.Card("Td"),
                new Poker.Card("Ac"),
                new Poker.Card("6d"),
                new Poker.Card("4s"),
                new Poker.Card("Tc"),
                new Poker.Card("2h")
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
                new Poker.Card("6d"),
                new Poker.Card("Kd"),
                new Poker.Card("6s"),
                new Poker.Card("2h"),
                new Poker.Card("Ah"),
                new Poker.Card("Kc")
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
                new Poker.Card("Ad"),
                new Poker.Card("Ac"),
                new Poker.Card("6s"),
                new Poker.Card("Ah"),
                new Poker.Card("2h"),
                new Poker.Card("3s")
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
                new Poker.Card("5s"),
                new Poker.Card("8d"),
                new Poker.Card("6c"),
                new Poker.Card("6h"),
                new Poker.Card("4h"),
                new Poker.Card("7c")
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
                new Poker.Card("4h"),
                new Poker.Card("Ad"),
                new Poker.Card("5c"),
                new Poker.Card("Tc"),
                new Poker.Card("2h"),
                new Poker.Card("3s")
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
                new Poker.Card("6d"),
                new Poker.Card("Ad"),
                new Poker.Card("Kd"),
                new Poker.Card("6h"),
                new Poker.Card("4d"),
                new Poker.Card("2d")
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
                new Poker.Card("Ad"),
                new Poker.Card("6h"),
                new Poker.Card("Ac"),
                new Poker.Card("6s"),
                new Poker.Card("8h"),
                new Poker.Card("6c")
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
                new Poker.Card("As"),
                new Poker.Card("Tc"),
                new Poker.Card("Ah"),
                new Poker.Card("Ad"),
                new Poker.Card("6c"),
                new Poker.Card("Ac")
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
                new Poker.Card("Js"),
                new Poker.Card("Ts"),
                new Poker.Card("8s"),
                new Poker.Card("9d"),
                new Poker.Card("Qs"),
                new Poker.Card("9s")
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
                    new Poker.Card("As"),
                    new Poker.Card("2d")
                },
                new List<Card>
                {
                    new Poker.Card("Ad"),
                    new Poker.Card("8d")
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
                    new Poker.Card("As"),
                    new Poker.Card("Kh")
                },
                new List<Card>
                {
                    new Poker.Card("9d"),
                    new Poker.Card("8d")
                },
                new List<Card>
                {
                    new Poker.Card("Ac"),
                    new Poker.Card("8s")
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
                    new Poker.Card("As"),
                    new Poker.Card("2h")
                },
                new List<Card>
                {
                    new Poker.Card("Ac"),
                    new Poker.Card("2s")
                }
            };
            var communityCards = new List<Card>
            {
                new Poker.Card("Ah"),
                new Poker.Card("Ad"),
                new Poker.Card("Kh"),
                new Poker.Card("Js"),
                new Poker.Card("Ts")
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
                    new Poker.Card("8s"),
                    new Poker.Card("4s")
                },
                new List<Card>
                {
                    new Poker.Card("Kh"),
                    new Poker.Card("Kc")
                }
            };
            var communityCards = new List<Card>
            {
                new Poker.Card("Ts"),
                new Poker.Card("2d"),
                new Poker.Card("6s"),
                new Poker.Card("9c")
            };

            var result = await Poker.Hand.EquityCalculator.Calc(hands, communityCards);
            var outs = new List<Card>
            {
                new Poker.Card("As"),
                new Poker.Card("Ks"),
                new Poker.Card("Qs"),
                new Poker.Card("Js"),
                new Poker.Card("9s"),
                new Poker.Card("7s"),
                new Poker.Card("5s"),
                new Poker.Card("3s"),
                new Poker.Card("2s"),
                new Poker.Card("7h"),
                new Poker.Card("7d"),
                new Poker.Card("7c")
            };

            Assert.AreEqual(outs.Count, result[0].Outs.Count);
            for(var i = 0; i < outs.Count; i++)
            {
                Assert.AreEqual(outs[i], result[0].Outs[i]);
            }
        }
    }
}
