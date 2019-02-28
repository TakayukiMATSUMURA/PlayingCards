using System.Linq;
using System.Collections.Generic;

namespace PlayingCards
{
    public static partial class BlackJack
    {
        public static List<Default.Card> Deck
        {
            get
            {
                return Default.Deck.Select(x => (Default.Card)new Card(x.Rank, x.Suit)).ToList();
            }
        }

        public class Card : Default.Card
        {
            public override int Rank => IsFace ? T : base.Rank;

            public Card(int rank, Suit suit) : base(rank, suit)
            {
            }
        }
    }
}
