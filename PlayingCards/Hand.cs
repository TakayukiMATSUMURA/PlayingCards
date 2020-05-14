using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace PlayingCards
{
    [DataContract]
    public class Hand<T> where T : Card
    {
        [IgnoreDataMember]
        public List<T> Cards { get; protected set; }

        [DataMember]
        protected List<string> _cards;

        public Hand()
        {
            Cards = new List<T>();
        }

        public Hand(List<T> cards)
        {
            Cards = new List<T>(cards);
        }

        public void Add(T card)
        {
            Cards.Add(card);
        }

        public override string ToString()
        {
            return string.Join("", Cards.Select(x => x.ToString()));
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _cards = Cards.Select(x => x.ToString()).ToList();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            var type = typeof(T);
            var ctor = type.GetConstructor(new Type[] { typeof(string) });
            Cards.AddRange(_cards.Select(x => ctor.Invoke(new object[] { x }) as T).ToList());
        }
    }
}

