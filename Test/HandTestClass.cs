using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using System.Threading.Tasks;

namespace PlayingCards.Poker.Test
{
    [TestFixture()]
    public class HandTestClass
    {
        [Test()]
        public void TestNoPair()
        {
            var cards = new List<Card>
            {
                new Card("Ad"),
                new Card("Kc"),
                new Card("6d"),
                new Card("4s"),
                new Card("2h")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.HighCard, hand.Rank);
        }

        [Test()]
        public void TestOnePair()
        {
            var cards = new List<Card>
            {
                new Card("Td"),
                new Card("Ac"),
                new Card("6d"),
                new Card("4s"),
                new Card("Tc"),
                new Card("2h")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.OnePair, hand.Rank);

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
                new Card("6d"),
                new Card("Kd"),
                new Card("6s"),
                new Card("2h"),
                new Card("Ah"),
                new Card("Kc")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.TwoPair, hand.Rank);

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
                new Card("Ad"),
                new Card("Ac"),
                new Card("6s"),
                new Card("Ah"),
                new Card("2h"),
                new Card("3s")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.ThreeOfAKind, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            cards.RemoveAt(4);
            cards.Sort();
            cards.Reverse();
            var aces = cards.Where(x => x.Rank == Card.A).ToList();
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
                new Card("5s"),
                new Card("8d"),
                new Card("6c"),
                new Card("6h"),
                new Card("4h"),
                new Card("7c")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.Straight, hand.Rank);

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
                new Card("4h"),
                new Card("Ad"),
                new Card("5c"),
                new Card("Tc"),
                new Card("2h"),
                new Card("3s")
            };
            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.Straight, hand.Rank);

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
                new Card("6d"),
                new Card("Ad"),
                new Card("Kd"),
                new Card("6h"),
                new Card("4d"),
                new Card("2d")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.Flush, hand.Rank);

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
                new Card("Ad"),
                new Card("6h"),
                new Card("Ac"),
                new Card("6s"),
                new Card("8h"),
                new Card("6c")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.FullHouse, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Rank == 6).ToList();
            usedCards.Sort();
            usedCards.Reverse();
            var aces = cards.Where(x => x.Rank == Card.A).ToList();
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
                new Card("As"),
                new Card("Tc"),
                new Card("Ah"),
                new Card("Ad"),
                new Card("6c"),
                new Card("Ac")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.FourOfAKind, hand.Rank);

            Assert.AreEqual(5, hand.Cards.Count);
            var usedCards = cards.Where(x => x.Rank == Card.A).ToList();
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
                new Card("Js"),
                new Card("Ts"),
                new Card("8s"),
                new Card("9d"),
                new Card("Qs"),
                new Card("9s")
            };

            var hand = new Hand(cards);
            Assert.AreEqual(HandRank.StraightFlush, hand.Rank);

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
                    new Card("As"),
                    new Card("2d")
                },
                new List<Card>
                {
                    new Card("Ad"),
                    new Card("8d")
                }
            };
            var communityCards = new List<Card> { };
            var result = await Equity.Calculator.Calc(hands, communityCards);
            Assert.AreEqual(32.97f, result[0].Equity.Total);
            Assert.AreEqual(22.73f, result[0].Equity.Win);
            Assert.AreEqual(20.49f, result[0].Equity.Split);
            Assert.AreEqual(67.03f, result[1].Equity.Total);
            Assert.AreEqual(56.78f, result[1].Equity.Win);
            Assert.AreEqual(20.49f, result[1].Equity.Split);
        }

        [Test()]
        public async Task TestEquityWith3Hands()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Card("As"),
                    new Card("Kh")
                },
                new List<Card>
                {
                    new Card("9d"),
                    new Card("8d")
                },
                new List<Card>
                {
                    new Card("Ac"),
                    new Card("8s")
                }
            };
            var communityCards = new List<Card> { };
            var result = await Equity.Calculator.Calc(hands, communityCards);
            Assert.AreEqual(55.28f, result[0].Equity.Total);
            Assert.AreEqual(53.32f, result[0].Equity.Win);
            Assert.AreEqual(4.02f, result[0].Equity.Split);
            Assert.AreEqual(31.18f, result[1].Equity.Total);
            Assert.AreEqual(30.34f, result[1].Equity.Win);
            Assert.AreEqual(1.79f, result[1].Equity.Split);
            Assert.AreEqual(13.54f, result[2].Equity.Total);
            Assert.AreEqual(10.85f, result[2].Equity.Win);
            Assert.AreEqual(5.48f, result[2].Equity.Split);
        }

        [Test()]
        public async Task TestEquityOnChop()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Card("As"),
                    new Card("2h")
                },
                new List<Card>
                {
                    new Card("Ac"),
                    new Card("2s")
                }
            };
            var communityCards = new List<Card>
            {
                new Card("Ah"),
                new Card("Ad"),
                new Card("Kh"),
                new Card("Js"),
                new Card("Ts")
            };

            var result = await Equity.Calculator.Calc(hands, communityCards);
            Assert.AreEqual(50f, result[0].Equity.Total);
            Assert.AreEqual(0, result[0].Equity.Win);
            Assert.AreEqual(100f, result[0].Equity.Split);
            Assert.AreEqual(50f, result[1].Equity.Total);
            Assert.AreEqual(0, result[1].Equity.Win);
            Assert.AreEqual(100f, result[1].Equity.Split);
        }

        [Test()]
        public async Task TestEquityWithExposedCards()
        {
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Card("As"),
                    new Card("Ks")
                },
                new List<Card>
                {
                    new Card("Th"),
                    new Card("9h")
                }
            };
            var communityCards = new List<Card>();
            var exposedCards = new List<Card>() { new Card("8h"), new Card("7h") };

            var result = await Equity.Calculator.Calc(hands, communityCards, exposedCards);
            Assert.AreEqual(64.42f, result[0].Equity.Total);
            Assert.AreEqual(64.17f, result[0].Equity.Win);
            Assert.AreEqual(0.5f, result[0].Equity.Split);
            Assert.AreEqual(35.58f, result[1].Equity.Total);
            Assert.AreEqual(35.33f, result[1].Equity.Win);
            Assert.AreEqual(0.5f, result[1].Equity.Split);
        }

        [Test()]
        public async Task TestOuts()
        {
            // WSOP 2019 last hand
            var hands = new List<List<Card>>
            {
                new List<Card>
                {
                    new Card("8s"),
                    new Card("4s")
                },
                new List<Card>
                {
                    new Card("Kh"),
                    new Card("Kc")
                }
            };
            var communityCards = new List<Card>
            {
                new Card("Ts"),
                new Card("2d"),
                new Card("6s"),
                new Card("9c")
            };

            var result = await Equity.Calculator.Calc(hands, communityCards);
            var outs = new List<Card>
            {
                new Card("As"),
                new Card("Ks"),
                new Card("Qs"),
                new Card("Js"),
                new Card("9s"),
                new Card("7s"),
                new Card("5s"),
                new Card("3s"),
                new Card("2s"),
                new Card("7h"),
                new Card("7d"),
                new Card("7c")
            };

            Assert.AreEqual(outs.Count, result[0].Outs.Count);
            for (var i = 0; i < outs.Count; i++)
            {
                Assert.AreEqual(outs[i], result[0].Outs[i]);
            }
        }
    }
}
