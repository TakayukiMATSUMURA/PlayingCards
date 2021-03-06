﻿using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using DefaultCard = PlayingCards.Card;

namespace PlayingCards.Poker
{
    [DataContract]
    public class Card : DefaultCard
    {
        public new const int A = 14;

        [IgnoreDataMember]
        public override int Rank => (base.Rank == DefaultCard.A ? A : base.Rank);

        public Card()
        {
        }

        public Card(int rank, Suit suit) : base(rank % 14, suit)
        {
        }

        [JsonConstructor]
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