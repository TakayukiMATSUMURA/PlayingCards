using System.Collections.Generic;
using System.Linq;

namespace PlayingCards
{
    public class Hand
    {
        public List<Card> Cards { get; } = new List<Card>();

        public Hand(params Card[] cards)
        {
            Cards = cards.ToList();
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
