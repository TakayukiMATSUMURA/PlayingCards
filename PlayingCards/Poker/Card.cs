using System.Linq;
using System.Collections.Generic;
using DefaultCard = PlayingCards.Card;

namespace PlayingCards.Poker
{
    public class Card : DefaultCard
    {
        public new const int A = 14;

        public override int Rank => (base.Rank == DefaultCard.A ? A : base.Rank);

        public Card(int rank, Suit suit) : base(rank % 14, suit)
        {
        }

        public Card(string card) : base(card)
        {
        }


        public new static List<Card> Deck
        {
            get
            {
                return DefaultCard.Deck.Select(x => new Card(x.Rank, x.Suit)).ToList();
            }
        }
    }
}