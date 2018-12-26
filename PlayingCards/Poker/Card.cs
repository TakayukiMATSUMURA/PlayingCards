﻿using System;
using System.Collections.Generic;

namespace PlayingCards.Poker
{
    public class Card : PlayingCards.Card
    {
        public new const int A = 14;

        public override int Rank => base.Rank == PlayingCards.Card.A ? A : base.Rank;

        public new static List<Card> GetDeck()
        {
            var result = new List<Card>();
            for (int r = PlayingCards.Card.A; r <= K; r++)
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