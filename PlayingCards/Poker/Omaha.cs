﻿using System.Collections.Generic;

namespace PlayingCards.Poker.Omaha
{
    using Card = PlayingCards.Card;

    public class Hand : Poker.Hand
    {
        public Hand(List<Card> pocketCards, List<Card> communityCards)
        {
            Code = 0;
            for (var i = 0; i < pocketCards.Count - 1; i++)
            {
                for (var j = i + 1; j < pocketCards.Count; j++)
                {
                    for (var k = 0; k < communityCards.Count - 2; k++)
                    {
                        for (var l = k + 1; l < communityCards.Count - 1; l++)
                        {
                            for (var m = l + 1; m < communityCards.Count; m++)
                            {

                                var cards = new List<Card> { pocketCards[i], pocketCards[j], communityCards[k], communityCards[l], communityCards[m] };
                                cards.Sort();
                                var hand = new Poker.Hand(cards);
                                if (hand.Code > Code)
                                {
                                    Cards = cards;
                                    Code = hand.Code;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}