using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlayingCards
{
    public enum Suit
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    [DataContract]
    public class Card : IComparable
    {
        public const int A = 1;
        public const int T = 10;
        public const int J = 11;
        public const int Q = 12;
        public const int K = 13;

        [IgnoreDataMember]
        private readonly int _rank;
        [IgnoreDataMember]
        public virtual int Rank => _rank;

        [IgnoreDataMember]
        public readonly Suit Suit;

        [IgnoreDataMember]
        public bool IsFace => _rank >= J;

        [DataMember(Name = "Card")]
        private string _card;

        public Card()
        {
        }

        public Card(int rank, Suit suit)
        {
            if (!(rank >= A && rank <= K))
            {
                throw new ArgumentException("Invalid card rank:" + rank);
            }
            _rank = rank;
            Suit = suit;
        }

        [JsonConstructor]
        public Card(string card)
        {
            var rank = card[0] == 'A' ? A :
                       card[0] == 'K' ? K :
                       card[0] == 'Q' ? Q :
                       card[0] == 'J' ? J :
                       card[0] == 'T' ? T : int.Parse(card[0].ToString());
            if (!(rank >= A && rank <= K))
            {
                throw new ArgumentException("Invalid card rank:" + card[0]);
            }

            if (!(card[1] == 'c' || card[1] == 'd' || card[1] == 'h' || card[1] == 's'))
            {
                throw new ArgumentException("Invalid card suit:" + card[1]);
            }
            var suit = card[1] == 'c' ? Suit.Club :
                       card[1] == 'd' ? Suit.Diamond :
                       card[1] == 'h' ? Suit.Heart : Suit.Spade;
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

        public int CompareTo(object obj)
        {
            var card = (Card)obj;
            return this < card ? -1 : this > card ? 1 : 0;
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

        public static List<Card> Deck
        {
            get
            {
                var result = new List<Card>();
                for (int r = A; r <= K; r++)
                {
                    foreach (Suit s in Enum.GetValues(typeof(Suit)))
                    {
                        result.Add(new Card(r, s));
                    }
                }

                return result;
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _card = ToString();
        }
    }
}
