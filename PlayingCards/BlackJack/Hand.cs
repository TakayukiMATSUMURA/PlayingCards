using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayingCards.BlackJack
{
    public class Hand
    {
        public Hand(params Card[] cards)
        {
            _cards = cards.ToList();
        }

        public bool IsBlackJack => _cards.Any(x => x.Rank == Card.A) && _cards.Any(x => x.Rank == Card.T);

        public bool IsSoft => !IsBlackJack && _cards.Any(x => x.Rank == Card.A) && _cards.Sum(x => x.Rank) <= 11;

        public int Rank => IsBlackJack ? 21 : IsSoft ? _cards.Sum(x => x.Rank) + 10 : _cards.Sum(x => x.Rank);

        public bool IsPair => _cards.Count == 2 && _cards[0].Rank == _cards[1].Rank;

        public List<Hand> Split()
        {
            if(!IsPair)
            {
                throw new Exception("Cannot Split:" + ToString());
            }

            var result = new List<Hand>();
            for (int i = 0; i < 2; i++)
            {
                result.Add(new Hand(_cards[i]));
            }
            return result;
        }

        public void Add(Card card)
        {
            _cards.Add(card);
        }

		public override string ToString()
        {
            return string.Join("", _cards.Select(x => x.ToString())) + ":" + Rank;
        }

		private readonly List<Card> _cards = new List<Card>();
    }
}
