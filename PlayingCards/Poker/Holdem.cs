using System.Collections.Generic;
using System.Linq;

namespace PlayingCards.Poker.Holdem
{
    using Card = PlayingCards.Card;

    public class Hand : Poker.Hand
    {
        public Hand(List<Card> pocketCards, List<Card> communityCards) : 
        base(new List<List<Card>> { pocketCards, communityCards }.SelectMany(x => x).ToList())
        {
        }
    }
}
