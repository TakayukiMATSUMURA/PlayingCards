using System.Collections.Generic;
using System.Linq;

namespace PlayingCards
{
    public class Hand
    {
        public List<Card> Cards { get; protected set; }

        public Hand()
        {
            Cards = new List<Card>();
        }

        public Hand(List<Card> cards)
        {
            Cards = cards;
        }

        public void Add(Card card)
        {
            Cards.Add(card);
        }

        public override string ToString()
        {
            return string.Join("", Cards.Select(x => x.ToString()));
        }
    }
}

