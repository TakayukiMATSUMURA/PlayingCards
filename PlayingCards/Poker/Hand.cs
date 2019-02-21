using System.Collections.Generic;

namespace PlayingCards.Poker
{
    public class Hand : PlayingCards.Hand
    {
        public enum Rank
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

        public ulong Code { get; protected set; }

        public Hand()
        {
        }

        public Hand(List<PlayingCards.Card> cards) : base(cards)
        {
            Cards.Sort();
            Code = Encode(Cards);
        }

        public override string ToString()
        {
            return string.Join("", base.ToString());
        }

        public static ulong Encode(List<PlayingCards.Card> cards)
        {
            ulong result = 0;
            uint bitmap = 0;
            uint mask = 0;
            var rcnt = new int[14];
            var scnt = new int[4];

            foreach (var card in cards)
            {
                rcnt[card.Rank + 1]++;
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
                        bitmap |= (uint)1 << c.Rank;
                    }
                }

                mask = 0x1f00; // AKQJT
                for (uint i = 0; i < 9; ++i, mask >>= 1)
                {
                    if ((bitmap & mask) == mask)
                    {
                        return (ulong)(13 - i) << 0x10 << ((int)(Rank.StraightFlush) * 4);
                    }
                }
                if (bitmap == 0x100f) // 1 0000 00000 1111 = A5432
                {
                    return (ulong)4 << 0x10 << ((int)(Rank.StraightFlush) * 4);
                }

                mask = 0x1000;
                for (int i = 0; i < Card.A; ++i, mask >>= 1)
                {
                    if ((bitmap & mask) != 0)
                    {
                        result = result << 4 | (uint)(13 - i);
                        if (result > 0x10000) break;
                    }
                }

                return result << ((int)(Rank.Flush) * 4);
            }

            var fourOfAKindIX = -1;
            var threeOfAKindIX1 = -1;
            var threeOfAKindIX2 = -1;
            var pairIX1 = -1;
            var pairIX2 = -1;
            for (int i = 1; i <= 13; ++i)
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
                result = (ulong)fourOfAKindIX;

                for (int i = 13; i > 0; i--)
                {
                    if (rcnt[i] > 0 && i != fourOfAKindIX)
                    {
                        result = (result << 4) | (uint)i;
                        return result << 12 << ((int)(Rank.FourOfAKind) * 4);
                    }
                }
            }

            if (threeOfAKindIX1 >= 0 && (threeOfAKindIX2 >= 0 || pairIX1 >= 0 || pairIX2 >= 0))
            {
                var pair = pairIX1 >= 0 ? pairIX1 : pairIX2;
                if (threeOfAKindIX2 > pair) pair = threeOfAKindIX2;

                result = (ulong)((threeOfAKindIX1 << 4) | pair);
                return result << 12 << ((int)(Rank.FullHouse) * 4);
            }

            bitmap = 0;
            mask = 1;
            for (int i = 1; i < Card.A; ++i, mask <<= 1)
            {
                if (rcnt[i] != 0)
                    bitmap |= mask;
            }
            mask = 0x1f00; // AKQJT

            for (int i = 0; i < 9; ++i, mask >>= 1)
            {
                if ((bitmap & mask) == mask)
                {
                    return (ulong)(13 - i) << 0x10 << ((int)(Rank.Straight) * 4);
                }
            }
            if ((bitmap & 0x100f) == 0x100f) // 1 0000 00000 1111 = A5432
            {
                return (ulong)4 << 0x10 << ((int)(Rank.Straight) * 4);
            }
            if (threeOfAKindIX1 >= 0)
            {
                result = (ulong)threeOfAKindIX1;

                for (int i = 13; i > 0; i--)
                {
                    if (rcnt[i] > 0 && i != threeOfAKindIX1)
                    {
                        result = (result << 4) | (uint)i;
                        if (result >= 0x100) break;
                    }
                }
                return (result << 8 << ((int)(Rank.ThreeOfAKind) * 4));
            }
            if (pairIX2 >= 0)
            {
                result = (ulong)((pairIX1 << 4) | pairIX2);

                for (int i = 13; i > 0; i--)
                {
                    if (rcnt[i] > 0 && i != pairIX2 && i != pairIX1)
                    {
                        result = (result << 4) | (uint)i;
                        break;
                    }
                }
                return (result << 8 << ((int)(Rank.TwoPair) * 4));
            }
            if (pairIX1 >= 0)
            {
                result = (ulong)pairIX1;

                for (int i = 13; i > 0; i--)
                {
                    if (rcnt[i] > 0 && i != pairIX1)
                    {
                        result = (result << 4) | (uint)i;
                        if (result >= 0x1000) break;
                    }
                }
                return (result << 4 << ((int)(Rank.OnePair) * 4));
            }

            for (uint i = 13; i > 0; i--)
            {
                if (rcnt[i] > 0)
                {
                    result = (result << 4) | i;
                    if (result >= 0x10000) break;
                }
            }
            return (result << ((int)Rank.HighCard * 4));
        }


        public override bool Equals(object obj)
        {
            return Code == ((Hand)obj).Code;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Hand a, Hand b)
        {
            return a.Code == b.Code;
        }
        public static bool operator !=(Hand a, Hand b)
        {
            return !(a == b);
        }

        public static bool operator <(Hand a, Hand b)
        {
            return a.Code < b.Code;
        }
        public static bool operator <=(Hand a, Hand b)
        {
            return a < b || a == b;
        }
        public static bool operator >(Hand a, Hand b)
        {
            return !(a.Code <= b.Code);
        }
        public static bool operator >=(Hand a, Hand b)
        {
            return !(a < b);
        }
    }
}
