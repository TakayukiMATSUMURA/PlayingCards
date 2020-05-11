using System.Linq;
using System.Collections.Generic;
using DefaultCard = PlayingCards.Card;

namespace PlayingCards.BlackJack
{
    public class Card : DefaultCard
    {
        public override int Rank => IsFace ? T : base.Rank;

        public Card(int rank, Suit suit) : base(rank, suit)
        {
        }

        public new static List<DefaultCard> Deck
        {
            get
            {
                return DefaultCard.Deck.Select(x => (DefaultCard)new Card(x.Rank, x.Suit)).ToList();
            }
        }
    }
}