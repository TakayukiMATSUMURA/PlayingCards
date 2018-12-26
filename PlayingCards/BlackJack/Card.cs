using System;
using System.Collections.Generic;

namespace PlayingCards.BlackJack
{
    public class Card : PlayingCards.Card
    {
        public override int Rank => IsFace ? T : base.Rank;

        public new static List<Card> GetDeck()
        {
            var result = new List<Card>();
            for (int r = 1; r <= K; r++)
            {
                foreach (Suit s in Enum.GetValues(typeof(Suit)))
                {
                    result.Add(new Card(r, s));
                }
            }

            return result;
        }

        public Card(int rank, Suit suit) : base(rank, suit)
        {
        }
	}
}
