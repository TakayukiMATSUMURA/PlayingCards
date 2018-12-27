using System.Collections.Generic;

namespace PlayingCards.Poker
{
    public class Hand : PlayingCards.Hand
    {
        public readonly ulong Code;

        public Hand(params PlayingCards.Card[] cards) : base(cards)
        {
            Cards.Sort();
        }

        public override string ToString()
        {
            return string.Join("", base.ToString());
        }

        public static ulong GetCode(List<PlayingCards.Card> cards)
        {
            return 0;
        }
    }
}
