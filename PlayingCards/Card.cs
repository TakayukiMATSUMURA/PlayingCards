using System;
using System.Collections.Generic;

namespace PlayingCards
{
    public enum Suit
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    public class Card
    {
        public const int A = 1;
        public const int T = 10;
        public const int J = 11;
        public const int Q = 12;
        public const int K = 13;

        private readonly int _rank;
        public virtual int Rank => _rank;

        public readonly Suit Suit;

        public bool IsFace => _rank >= J;

        public static List<Card> GetDeck()
        {
            var result = new List<Card>();
            for (int r = A; r <= K; r++)
            {
                foreach(Suit s in Enum.GetValues(typeof(Suit)))
                {
                    result.Add(new Card(r, s));
                }
            }

            return result;
        }

        public Card(int rank, Suit suit)
        {
            if(!(rank >= A && rank <= K))
            {
                throw new ArgumentException("Invalid card rank:" + rank);
            }
            _rank = rank;
            Suit = suit;
        }

		public override string ToString()
		{
            var result = _rank == A ? "A" :
                         _rank == T ? "T" :
                         _rank == J ? "J" :
                         _rank == Q ? "Q" :
                         _rank == K ? "K" : Rank.ToString();
            result += Suit.ToString().ToLower()[0];

            return result;
		}

        public override bool Equals(object obj)
        {
            return this == (Card)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Card a, Card b)
        {
            return a.Rank == b.Rank && a.Suit == b.Suit;
        }
        public static bool operator !=(Card a, Card b)
        {
            return !(a == b);
        }

        public static bool operator <(Card a, Card b)
        {
            return a.Rank < b.Rank || (a.Rank == b.Rank && a.Suit < b.Suit);
        }
        public static bool operator <=(Card a, Card b)
        {
            return a < b || a == b;
        }
        public static bool operator >(Card a, Card b)
        {
            return !(a <= b);
        }
        public static bool operator >=(Card a, Card b)
        {
            return !(a < b);
        }
    }
}
