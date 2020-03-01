using System.Linq;
using System.Collections.Generic;

namespace PlayingCards
{
    public static partial class Poker
    {
        public enum HandRank
        {
            HighCard = 0,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush
        }

        public partial class Hand : Default.Hand
        {
            protected ulong _code;
            public HandRank Rank => Cards.Count >= 5 ? GetRank(_code) : HandRank.HighCard;

            public readonly List<Default.Card> PocketCards;
            public readonly List<Default.Card> CommunityCards;

            public float Equity { get; private set; } = -1;

            public Hand()
            {
            }

            public Hand(List<Default.Card> pocketCards, List<Default.Card> communityCards)
            {
                if(pocketCards[0] < pocketCards[1])
                {
                    pocketCards.Add(pocketCards[0]);
                    pocketCards.RemoveAt(0);
                }
                PocketCards = pocketCards;
                CommunityCards = communityCards;
                Update();
            }

            public override string ToString()
            {
                return base.ToString() + " " + Rank.ToString() + (Equity > 0 ? $" {Equity}%" : "");
            }

            public void Update()
            {
                var cards = new List<List<Default.Card>> { PocketCards, CommunityCards }.SelectMany(x => x).ToList();
                if (cards.Count >= 5)
                {
                    _code = Encode(cards);
                    Cards = GetUsedCards(_code, cards);
                }
                else
                {
                    _code = 0;
                    Cards = cards;
                }
            }

            public static ulong Encode(List<Default.Card> cards, int[] rcnt = null, int[] scnt = null)
            {
                ulong result = 0;
                uint bitmap = 0;
                uint mask = 0;
                if (rcnt == null)
                {
                    rcnt = new int[14];
                    scnt = new int[4];

                    foreach (var card in cards)
                    {
                        rcnt[card.Rank - 2]++;
                        scnt[(int)card.Suit]++;
                    }
                }

                var s = -1;
                if (scnt[s = 0] >= 5 || scnt[s = 1] >= 5 || scnt[s = 2] >= 5 || scnt[s = 3] >= 5)
                {
                    // フラッシュ確定
                    bitmap = 0;
                    for (var i = 0; i < cards.Count; ++i)
                    {
                        var c = cards[i];
                        if ((int)c.Suit == s)
                        {
                            bitmap |= (uint)1 << (c.Rank - 2);
                        }
                    }

                    mask = 0x1f00; // AKQJT
                    for (uint i = 0; i < 9; ++i, mask >>= 1)
                    {
                        if ((bitmap & mask) == mask)
                        {
                            return (ulong)(14 - i) << (4 * 4) << ((int)(HandRank.StraightFlush) * 4);
                        }
                    }

                    mask = 0x100f;
                    if ((bitmap & mask) == mask) // 1 0000 00000 1111 = A5432
                    {
                        return (ulong)5 << 0x10 << ((int)(HandRank.StraightFlush) * 4);
                    }

                    mask = 0x1000;
                    for (int i = 0; i < Card.A; ++i, mask >>= 1)
                    {
                        if ((bitmap & mask) != 0)
                        {
                            result = result << 4 | (uint)(Poker.Card.A - i);
                            if (result > 0x10000) break;
                        }
                    }

                    return result << ((int)(HandRank.Flush) * 4);
                }

                var fourOfAKindIX = -1;
                var threeOfAKindIX1 = -1;
                var threeOfAKindIX2 = -1;
                var pairIX1 = -1;
                var pairIX2 = -1;
                for (int i = 0; i < 13; ++i)
                {
                    switch (rcnt[i])
                    {
                        case 4:
                            fourOfAKindIX = i;
                            break;
                        case 3:
                            threeOfAKindIX2 = threeOfAKindIX1;
                            threeOfAKindIX1 = i;
                            break;
                        case 2:
                            pairIX2 = pairIX1;
                            pairIX1 = i;
                            break;
                    }
                }

                if (fourOfAKindIX >= 0)
                {
                    result = (ulong)fourOfAKindIX + 2;

                    for (int i = 12; i >= 0; i--)
                    {
                        if (rcnt[i] > 0 && i != fourOfAKindIX)
                        {
                            result = (result << 4) | (uint)(i + 2);
                            return result << 12 << ((int)(HandRank.FourOfAKind) * 4);
                        }
                    }
                }

                if (threeOfAKindIX1 >= 0 && (threeOfAKindIX2 >= 0 || pairIX1 >= 0 || pairIX2 >= 0))
                {
                    var pair = pairIX1 >= 0 ? pairIX1 : pairIX2;
                    if (threeOfAKindIX2 > pair) pair = threeOfAKindIX2;

                    result = (ulong)(((threeOfAKindIX1 + 2) << 4) | (pair + 2));
                    return result << 12 << ((int)(HandRank.FullHouse) * 4);
                }

                bitmap = 0;
                mask = 1;
                for (int i = 0; i < 13; ++i, mask <<= 1)
                {
                    if (rcnt[i] != 0)
                        bitmap |= mask;
                }
                mask = 0x1f00; // AKQJT

                for (int i = 0; i < 9; ++i, mask >>= 1)
                {
                    if ((bitmap & mask) == mask)
                    {
                        return (ulong)(14 - i) << 0x10 << ((int)(HandRank.Straight) * 4);
                    }
                }
                if ((bitmap & 0x100f) == 0x100f) // 1 0000 00000 1111 = A5432
                {
                    return (ulong)5 << 0x10 << ((int)(HandRank.Straight) * 4);
                }
                if (threeOfAKindIX1 >= 0)
                {
                    result = (ulong)threeOfAKindIX1 + 2;

                    for (int i = 12; i >= 0; i--)
                    {
                        if (rcnt[i] > 0 && i != threeOfAKindIX1)
                        {
                            result = (result << 4) | (uint)(i + 2);
                            if (result >= 0x100) break;
                        }
                    }
                    return (result << 8 << ((int)(HandRank.ThreeOfAKind) * 4));
                }
                if (pairIX2 >= 0)
                {
                    result = (ulong)(((pairIX1 + 2) << 4) | (pairIX2 + 2));

                    for (int i = 12; i >= 0; i--)
                    {
                        if (rcnt[i] > 0 && i != pairIX2 && i != pairIX1)
                        {
                            result = (result << 4) | (uint)(i + 2);
                            break;
                        }
                    }
                    return (result << 8 << ((int)(HandRank.TwoPair) * 4));
                }
                if (pairIX1 >= 0)
                {
                    result = (ulong)(pairIX1 + 2);

                    for (int i = 12; i >= 0; i--)
                    {
                        if (rcnt[i] > 0 && i != pairIX1)
                        {
                            result = (result << 4) | (uint)(i + 2);
                            if (result >= 0x1000) break;
                        }
                    }
                    return (result << 4 << ((int)(HandRank.OnePair) * 4));
                }

                for (int i = 12; i >= 0; i--)
                {
                    if (rcnt[i] > 0)
                    {
                        result = (result << 4) | (uint)(i + 2);
                        if (result >= 0x10000) break;
                    }
                }
                return (result << ((int)HandRank.HighCard * 4));
            }

            public static HandRank GetRank(ulong code)
            {
                return code >= ((ulong)0x10000 << (int)HandRank.StraightFlush * 4) ? HandRank.StraightFlush :
                       code >= ((ulong)0x10000 << (int)HandRank.FourOfAKind * 4) ? HandRank.FourOfAKind :
                       code >= ((ulong)0x10000 << (int)HandRank.FullHouse * 4) ? HandRank.FullHouse :
                       code >= ((ulong)0x10000 << (int)HandRank.Flush * 4) ? HandRank.Flush :
                       code >= ((ulong)0x10000 << (int)HandRank.Straight * 4) ? HandRank.Straight :
                       code >= ((ulong)0x10000 << (int)HandRank.ThreeOfAKind * 4) ? HandRank.ThreeOfAKind :
                       code >= ((ulong)0x10000 << (int)HandRank.TwoPair * 4) ? HandRank.TwoPair :
                       code >= ((ulong)0x10000 << (int)HandRank.OnePair * 4) ? HandRank.OnePair : HandRank.HighCard;
            }

            public static List<Default.Card> GetUsedCards(ulong code, List<Default.Card> allCards)
            {
                var cards = new List<Default.Card>(allCards);
                cards.Sort();
                cards.Reverse();
                var result = new List<Default.Card>();
                var rank = GetRank(code);
                var cardsCode = code >> ((int)rank * 4);
                var nextRank = (int)(cardsCode >> (4 * 4));
                Suit majorSuit;
                var majorRank = 0;
                var pair = new List<Default.Card>();
                switch (rank)
                {
                    case HandRank.StraightFlush:
                        majorSuit = cards.Select(x => x.Suit).First(x => cards.Count(y => y.Suit == x) >= 5);
                        var majorSuitCards = cards.Where(x => x.Suit == majorSuit).ToList();
                        while (result.Count < 5)
                        {
                            result.Add(majorSuitCards.First(x => x.Rank == nextRank));
                            nextRank--;
                            if (nextRank == 1)
                            {
                                nextRank = Card.A;
                            }
                        }
                        return result;
                    case HandRank.FourOfAKind:
                        majorRank = (int)(cardsCode >> (4 * 4));
                        result = cards.Where(x => x.Rank == majorRank).ToList();
                        result.Add(cards.First(x => x.Rank == (int)((cardsCode >> (4 * 3)) & 0xf)));
                        return result;
                    case HandRank.FullHouse:
                        majorRank = (int)(cardsCode >> (4 * 4));
                        result = cards.Where(x => x.Rank == majorRank).ToList();
                        result.Sort();
                        result.Reverse();
                        majorRank = (int)(cardsCode >> (4 * 3)) & 0xf;
                        pair = cards.Where(x => x.Rank == majorRank).ToList();
                        pair.Sort();
                        pair.Reverse();
                        result.AddRange(pair);
                        return result;
                    case HandRank.Flush:
                        majorSuit = cards.Select(x => x.Suit).First(x => cards.Count(y => y.Suit == x) >= 5);
                        result = cards.Where(x => x.Suit == majorSuit).ToList();
                        result.Sort();
                        result.Reverse();
                        return result.Take(5).ToList();
                    case HandRank.Straight:
                        while (result.Count < 5)
                        {
                            result.Add(cards.First(x => x.Rank == nextRank));
                            nextRank--;
                            if (nextRank == 1)
                            {
                                nextRank = Card.A;
                            }
                        }
                        return result;
                    case HandRank.ThreeOfAKind:
                        majorRank = (int)(cardsCode >> (4 * 4));
                        result = cards.Where(x => x.Rank == majorRank).ToList();
                        result.Sort();
                        result.Reverse();
                        while (result.Count < 5)
                        {
                            result.Add(cards.First(x => !result.Contains(x)));
                        }
                        return result;
                    case HandRank.TwoPair:
                        majorRank = (int)(cardsCode >> (4 * 4));
                        result = cards.Where(x => x.Rank == majorRank).ToList();
                        result.Sort();
                        result.Reverse();
                        majorRank = (int)(cardsCode >> (4 * 3)) & 0xf;
                        pair = cards.Where(x => x.Rank == majorRank).ToList();
                        pair.Sort();
                        pair.Reverse();
                        result.AddRange(pair);
                        result.Add(cards.First(x => !result.Contains(x)));
                        return result;
                    case HandRank.OnePair:
                        majorRank = (int)(cardsCode >> (4 * 4));
                        result = cards.Where(x => x.Rank == majorRank).ToList();
                        result.AddRange(cards.Where(x => !result.Contains(x)).Take(3));
                        return result;
                    default:
                        return cards.Take(5).ToList();
                }
            }

            public override bool Equals(object obj)
            {
                return _code == ((Hand)obj)._code;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(Hand a, Hand b)
            {
                if (ReferenceEquals(a, b))
                {
                    return true;
                }

                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                return a._code == b?._code;
            }
            public static bool operator !=(Hand a, Hand b)
            {
                return !(a == b);
            }

            public static bool operator <(Hand a, Hand b)
            {
                return a._code < b._code;
            }
            public static bool operator <=(Hand a, Hand b)
            {
                return a < b || a == b;
            }
            public static bool operator >(Hand a, Hand b)
            {
                return !(a._code <= b._code);
            }
            public static bool operator >=(Hand a, Hand b)
            {
                return !(a < b);
            }
        }
    }
}