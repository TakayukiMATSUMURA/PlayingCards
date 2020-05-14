using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using DefaultCard = PlayingCards.Card;

namespace PlayingCards.BlackJack
{
    [DataContract]
    public class Card : DefaultCard
    {
        [IgnoreDataMember]
        public override int Rank => IsFace ? T : base.Rank;

        public Card(int rank, Suit suit) : base(rank, suit)
        {
        }

        [JsonConstructor]
        public Card(string card) : base(card)
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