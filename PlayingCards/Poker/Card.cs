using System.Linq;
using System.Collections.Generic;

namespace PlayingCards
{
    public static partial class Poker
    {
        public class Card : Default.Card
        {
            public new const int A = 14;

            public override int Rank => (base.Rank == Default.Card.A ? A : base.Rank);

            public Card(int rank, Suit suit) : base(rank % 14, suit)
            {
            }

            public Card(string card) : base(card)
            {
            }
        }

        public static List<Default.Card> Deck
        {
            get
            {
                return Default.Deck.Select(x => (Default.Card)new Card(x.Rank, x.Suit)).ToList();
            }
        }
    }
}