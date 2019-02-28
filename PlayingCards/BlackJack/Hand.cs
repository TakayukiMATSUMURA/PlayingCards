using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayingCards
{
    public static partial class BlackJack
    {
        public class Hand : Default.Hand
        {
            public Hand(List<Default.Card> cards) : base(cards)
            {
            }

            public bool IsBlackJack => Cards.Any(x => x.Rank == Card.A) && Cards.Any(x => x.Rank == Card.T);

            public bool IsSoft => !IsBlackJack && Cards.Any(x => x.Rank == Card.A) && Cards.Sum(x => x.Rank) <= 11;

            public int Rank => IsBlackJack ? 21 : IsSoft ? Cards.Sum(x => x.Rank) + 10 : Cards.Sum(x => x.Rank);

            public bool IsPair => Cards.Count == 2 && Cards[0].Rank == Cards[1].Rank;

            public List<Hand> Split()
            {
                if (!IsPair)
                {
                    throw new Exception("Cannot Split:" + ToString());
                }

                var result = new List<Hand>();
                for (int i = 0; i < 2; i++)
                {
                    result.Add(new Hand(new List<Default.Card> { Cards[i] }));
                }
                return result;
            }

            public override string ToString()
            {
                return string.Join("", base.ToString()) + ":" + Rank;
            }
        }
    }
}
