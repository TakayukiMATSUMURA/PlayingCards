using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PlayingCards.Poker
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

    public class Hand : PlayingCards.Hand
    {
        protected ulong _code { get; set; }
        public HandRank Rank => GetRank(_code);

        public Hand()
        {
        }

        public Hand(List<PlayingCards.Card> cards)
        {
            _code = Encode(cards);
            Cards = GetUsedCards(_code, cards);
        }

        public override string ToString()
        {
            return base.ToString() + " " + Rank.ToString();
        }

        public static ulong Encode(List<PlayingCards.Card> cards)
        {
            ulong result = 0;
            uint bitmap = 0;
            uint mask = 0;
            var rcnt = new int[13];
            var scnt = new int[4];

            foreach (var card in cards)
            {
                rcnt[card.Rank - 2]++;
                scnt[(int)card.Suit]++;
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
                if (bitmap == 0x100f) // 1 0000 00000 1111 = A5432
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

            for (uint i = 12; i >= 0; i--)
            {
                if (rcnt[i] > 0)
                {
                    result = (result << 4) | (i + 2);
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

        public static List<PlayingCards.Card> GetUsedCards(ulong code, List<PlayingCards.Card> allCards)
        {
            var cards = new List<PlayingCards.Card>(allCards);
            cards.Sort();
            cards.Reverse();
            var result = new List<PlayingCards.Card>();
            var rank = GetRank(code);
            var cardsCode = code >> ((int)rank * 4);
            var nextRank = (int)(cardsCode >> (4 * 4));
            Suit majorSuit;
            var majorRank = 0;
            var pair = new List<PlayingCards.Card>();
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

        public class Equity
        {
            public float Total;
            public float Win;
            public float Split;
        }

        private static Task<(int[], int[,])> CreateWorker(List<List<PlayingCards.Card>> hands, List<PlayingCards.Card> communityCards, List<PlayingCards.Card> deck, BlockingCollection<int> indexes)
        {
            return Task.Run(() =>
            {
                var comm = new List<PlayingCards.Card>(communityCards);
                var restCards = 5 - comm.Count;
                var counters = new int[hands.Count];
                var splitCounters = new int[hands.Count, hands.Count + 1];

                int deckIndexes = 0;
                while (true)
                {
                    deckIndexes = indexes.Take();
                    if (deckIndexes == -1)
                    {
                        break;
                    }

                    for (var i = 0; i < restCards; i++)
                    {
                        comm.Add(deck[deckIndexes & 0x3f]);
                        deckIndexes >>= 6;
                    }

                    var winners = GetWinners(hands, comm);
                    if ((winners & 0x8000) == 0)
                    {
                        var index = 0;
                        while (winners % 2 == 0)
                        {
                            index++;
                            winners >>= 1;
                        }
                        counters[index]++;
                    }
                    else
                    {
                        winners &= 0x7fff;
                        var numPlayer = 0;
                        var tmp = winners;
                        while (tmp > 0)
                        {
                            if (tmp % 2 == 1)
                            {
                                numPlayer++;
                            }
                            tmp >>= 1;
                        }

                        var index = 0;
                        while (winners > 0)
                        {
                            if ((winners % 2) != 0)
                            {
                                splitCounters[index, numPlayer]++;
                            }

                            index++;
                            winners >>= 1;
                        }
                    }

                    for (var i = 0; i < restCards; i++)
                    {
                        comm.RemoveAt(comm.Count - 1);
                    }
                };

                return (counters, splitCounters);
            });
        }

        public static async Task<List<Equity>> CalcEquity(List<List<PlayingCards.Card>> hands, List<PlayingCards.Card> communityCards)
        {
            var result = new List<Equity>();

            var deck = Card.GetDeck();
            var allCards = hands.SelectMany(x => x);
            deck.RemoveAll(x => allCards.Contains(x) || communityCards.Contains(x));
            var counters = new int[hands.Count];
            var splitCounters = new int[hands.Count, hands.Count + 1];

            var times = 0;
            var restCards = 5 - communityCards.Count;
            var workerNum = 8;
            var workers = new List<Task<(int[], int[,])>>();
            var blockingCollections = new List<BlockingCollection<int>>();
            for (var i = 0; i < workerNum; i++)
            {
                var collection = new BlockingCollection<int>();
                blockingCollections.Add(collection);
                workers.Add(CreateWorker(hands, communityCards, deck, collection));
            }

            var workerIndex = 0;
            if (restCards == 1)
            {
                for (var i = 0; i < deck.Count; i++)
                {
                    blockingCollections[workerIndex].Add(i);
                    workerIndex = (workerIndex + 1) % workerNum;
                    times++;
                }
                foreach(var collection in blockingCollections)
                {
                    collection.Add(-1);
                }
            }
            else if (restCards == 2)
            {
                for (var i = 0; i < deck.Count - 1; i++)
                {
                    for (var j = i + 1; j < deck.Count; j++)
                    {
                        blockingCollections[workerIndex].Add(j << 6 | i);
                        workerIndex = (workerIndex + 1) % workerNum;
                        times++;
                    }
                }
                foreach (var collection in blockingCollections)
                {
                    collection.Add(-1);
                }
            }
            else if (restCards == 5)
            {
                for (var i = 0; i < deck.Count - 4; i++)
                {
                    for (var j = i + 1; j < deck.Count - 3; j++)
                    {
                        for (var k = j + 1; k < deck.Count - 2; k++)
                        {
                            for (var l = k + 1; l < deck.Count - 1; l++)
                            {
                                for (var m = l + 1; m < deck.Count; m++)
                                {
                                    blockingCollections[workerIndex].Add(m << 24 | l << 18 | k << 12 | j << 6 | i);
                                    workerIndex = (workerIndex + 1) % workerNum;
                                    times++;
                                }
                            }
                        }
                    }
                }

                foreach (var collection in blockingCollections)
                {
                    collection.Add(-1);
                }
            }

            foreach(var worker in workers)
            {
                var (win, split) = await worker;
                for(var i = 0; i < hands.Count; i++)
                {
                    counters[i] += win[i];
                }
                for(var i = 0; i < hands.Count; i++)
                {
                    for(var j = 0; j < hands.Count + 1; j++)
                    {
                        splitCounters[i, j] += split[i, j];
                    }
                }
            }

            for (var i = 0; i < hands.Count; i++)
            {
                var win = (float)counters[i] * 100 / times;
                var split = 0.0f;
                for (var j = 1; j <= hands.Count; j++)
                {
                    split += splitCounters[i, j];
                }
                split = split * 100 / times;
                var equ = 0.0f;
                for (var j = 2; j <= hands.Count; j++)
                {
                    equ += splitCounters[i, j] / j;
                }
                equ = equ * 100 / times + win;
                result.Add(new Equity { Total = (float)Math.Round(equ, 2), Win = (float)Math.Round(win, 2), Split = (float)Math.Round(split, 2) });
            }

            return result;
        }

        private static int GetWinners(List<List<PlayingCards.Card>> hands, List<PlayingCards.Card> communityCards)
        {
            var codes = GetHandCodes(hands, communityCards);
            var maxCode = codes[0];
            var result = 1;
            for (var i = 1; i < codes.Count; i++)
            {
                if (codes[i] == maxCode)
                {
                    result |= 0x8000 | (1 << i);
                }
                else if (codes[i] > maxCode)
                {
                    result = 1 << i;
                    maxCode = codes[i];
                }
            }

            return result;
        }

        private static List<ulong> GetHandCodes(List<List<PlayingCards.Card>> hands, List<PlayingCards.Card> communityCards)
        {
            var communityCardSize = communityCards.Count;
            communityCards.Add(hands[0][0]);
            communityCards.Add(hands[0][1]);
            var result = new List<ulong>();

            foreach (var hand in hands)
            {
                for (var i = 0; i < 2; i++)
                {
                    communityCards[communityCardSize + i] = hand[i];
                }
                result.Add(Encode(communityCards));
            }

            communityCards.RemoveRange(communityCardSize, 2);

            return result;
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
