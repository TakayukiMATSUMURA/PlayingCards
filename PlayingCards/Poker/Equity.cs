using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PlayingCards.Poker
{
    [DataContract]
    public class Equity
    {
        [DataMember]
        public readonly float Total;
        [DataMember]
        public readonly float Win;
        [DataMember]
        public readonly float Split;

        public Equity(float total, float win, float split)
        {
            Total = total;
            Win = win;
            Split = split;
        }

        public override string ToString()
        {
            return $"equ:{Total}%\nwin:{Win}%\nsplit:{Split}%";
        }

        public static class Calculator
        {
            public class Result
            {
                public Equity Equity;
                public List<Card> Outs = new List<Card>();
            }

            public static async Task Calc(List<Hand> hands, List<Card> communityCards, List<Card> exposedCards = null)
            {
                var cards = hands.Select(x => x.PocketCards).ToList();
                var result = await Calc(cards, communityCards, exposedCards);
                for (var i = 0; i < hands.Count; i++)
                {
                    hands[i].SetCalculateResult(result[i]);
                }
            }

            public static async Task<List<Result>> Calc(List<List<Card>> hands, List<Card> communityCards, List<Card> exposedCards = null)
            {
                var result = new List<Result>();

                var deck = Card.Deck;
                var allCards = hands.SelectMany(x => x);
                deck.RemoveAll(x => allCards.Contains(x) || communityCards.Contains(x) || (exposedCards != null && exposedCards.Contains(x)));
                var counters = new int[hands.Count];
                var splitCounters = new int[hands.Count, hands.Count + 1];

                var times = 0;
                var restCards = 5 - communityCards.Count;
                var workerNum = 8;
                var workers = new List<Task<(int[], int[,])>>();
                var blockingCollections = new List<BlockingCollection<int>>();
                for (var i = 0; i < workerNum; i++)
                {
                    var collection = new BlockingCollection<int>();
                    blockingCollections.Add(collection);
                    workers.Add(CreateWorker(hands, communityCards, deck, collection));
                }

                var workerIndex = 0;
                if (restCards == 0)
                {
                    blockingCollections[workerIndex].Add(int.MaxValue);
                    times++;
                    foreach (var collection in blockingCollections)
                    {
                        collection.Add(-1);
                    }
                }
                if (restCards == 1)
                {
                    for (var i = 0; i < deck.Count; i++)
                    {
                        blockingCollections[workerIndex].Add(i);
                        workerIndex = (workerIndex + 1) % workerNum;
                        times++;
                    }
                    foreach (var collection in blockingCollections)
                    {
                        collection.Add(-1);
                    }
                }
                else if (restCards == 2)
                {
                    for (var i = 0; i < deck.Count - 1; i++)
                    {
                        for (var j = i + 1; j < deck.Count; j++)
                        {
                            blockingCollections[workerIndex].Add(j << 6 | i);
                            workerIndex = (workerIndex + 1) % workerNum;
                            times++;
                        }
                    }
                    foreach (var collection in blockingCollections)
                    {
                        collection.Add(-1);
                    }
                }
                else if (restCards == 5)
                {
                    for (var i = 0; i < deck.Count - 4; i++)
                    {
                        for (var j = i + 1; j < deck.Count - 3; j++)
                        {
                            for (var k = j + 1; k < deck.Count - 2; k++)
                            {
                                for (var l = k + 1; l < deck.Count - 1; l++)
                                {
                                    for (var m = l + 1; m < deck.Count; m++)
                                    {
                                        blockingCollections[workerIndex].Add(m << 24 | l << 18 | k << 12 | j << 6 | i);
                                        workerIndex = (workerIndex + 1) % workerNum;
                                        times++;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var collection in blockingCollections)
                    {
                        collection.Add(-1);
                    }
                }

                foreach (var worker in workers)
                {
                    var (win, split) = await worker;
                    for (var i = 0; i < hands.Count; i++)
                    {
                        counters[i] += win[i];
                    }
                    for (var i = 0; i < hands.Count; i++)
                    {
                        for (var j = 0; j < hands.Count + 1; j++)
                        {
                            splitCounters[i, j] += split[i, j];
                        }
                    }
                }

                for (var i = 0; i < hands.Count; i++)
                {
                    var win = (float)counters[i] * 100 / times;
                    var split = 0.0f;
                    for (var j = 1; j <= hands.Count; j++)
                    {
                        split += splitCounters[i, j];
                    }
                    split = split * 100 / times;
                    var equ = 0.0f;
                    for (var j = 2; j <= hands.Count; j++)
                    {
                        equ += splitCounters[i, j] * 100 / j;
                    }
                    equ = equ / times + win;
                    result.Add(new Result { Equity = new Equity((float)Math.Round(equ, 2), (float)Math.Round(win, 2), (float)Math.Round(split, 2)) });
                }

                // アウツの計算
                if (communityCards.Count == 3 || communityCards.Count == 4)
                {
                    var currentWinners = GetWinners(hands, communityCards);
                    var currentWinner = 0;
                    while ((currentWinners & 0x1) == 0)
                    {
                        currentWinner++;
                        currentWinners = currentWinners >> 1;
                    }

                    if ((currentWinner & 0x8000) != 0) return result;

                    for (var i = deck.Count - 1; i >= 0; i--)
                    {
                        var candidate = deck[i];
                        communityCards.Add(candidate);
                        var winner = GetWinners(hands, communityCards);
                        communityCards.Remove(candidate);

                        if ((winner & 0x8000) == 0)
                        {
                            var index = 0;
                            while ((winner & 0x1) == 0)
                            {
                                index++;
                                winner = winner >> 1;
                            }

                            if (currentWinner != index)
                            {
                                result[index].Outs.Add(candidate);
                            }
                        }
                    }

                    // アウツのソート(強い役になるものから順に並べる)
                    for (var i = 0; i < hands.Count; i++)
                    {
                        var hand = hands[i];
                        if (result[i].Outs.Any())
                        {
                            result[i].Outs.Sort((x, y) =>
                            {
                                var cards = new List<Card>(communityCards);
                                cards.AddRange(hand);
                                cards.Add(x);
                                var code0 = Hand.Encode(cards);
                                cards.Remove(x);
                                cards.Add(y);
                                var code1 = Hand.Encode(cards);
                                return code1 > code0 ? 1 : code1 == code0 ? x > y ? -1 : 1 : -1;
                            });
                        }
                    }
                }

                return result;
            }

            private static Task<(int[], int[,])> CreateWorker(List<List<Card>> hands, List<Card> communityCards, List<Card> deck, BlockingCollection<int> indexes)
            {
                return Task.Run(() =>
                {
                    var comm = new List<Card>(communityCards);
                    var commSize = communityCards.Count;
                    var restCards = 5 - comm.Count;
                    for (var i = 0; i < restCards; i++)
                    {
                        comm.Add(deck[0]);
                    }
                    for (var i = 0; i < 2; i++)
                    {
                        comm.Add(deck[0]);
                    }
                    var counters = new int[hands.Count];
                    var splitCounters = new int[hands.Count, hands.Count + 1];

                    int deckIndexes = 0;
                    while (true)
                    {
                        deckIndexes = indexes.Take();
                        if (deckIndexes == -1)
                        {
                            break;
                        }

                        for (var i = 0; i < restCards; i++)
                        {
                            comm[commSize + i] = deck[deckIndexes & 0x3f];
                            deckIndexes >>= 6;
                        }

                        var winners = GetWinners(hands, comm);
                        if ((winners & 0x8000) == 0)
                        {
                            var index = 0;
                            while (winners % 2 == 0)
                            {
                                index++;
                                winners >>= 1;
                            }
                            counters[index]++;
                        }
                        else
                        {
                            winners &= 0x7fff;
                            var numPlayer = 0;
                            var tmp = winners;
                            while (tmp > 0)
                            {
                                if (tmp % 2 == 1)
                                {
                                    numPlayer++;
                                }
                                tmp >>= 1;
                            }

                            var index = 0;
                            while (winners > 0)
                            {
                                if ((winners % 2) != 0)
                                {
                                    splitCounters[index, numPlayer]++;
                                }

                                index++;
                                winners >>= 1;
                            }
                        }
                    };

                    return (counters, splitCounters);
                });
            }

            private static int GetWinners(List<List<Card>> hands, List<Card> communityCards)
            {
                var codes = GetHandCodes(hands, communityCards);
                var maxCode = codes[0];
                var result = 1;
                for (var i = 1; i < codes.Count; i++)
                {
                    if (codes[i] == maxCode)
                    {
                        result |= 0x8000 | (1 << i);
                    }
                    else if (codes[i] > maxCode)
                    {
                        result = 1 << i;
                        maxCode = codes[i];
                    }
                }

                return result;
            }

            private static List<ulong> GetHandCodes(List<List<Card>> hands, List<Card> communityCards)
            {
                var result = new List<ulong>();
                foreach (var hand in hands)
                {
                    if (communityCards.Count == 7)
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            communityCards[5 + i] = hand[i];
                        }
                        result.Add(Hand.Encode(communityCards));
                    }
                    else
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            communityCards.Add(hand[i]);
                        }
                        result.Add(Hand.Encode(communityCards));
                        for (var i = 0; i < 2; i++)
                        {
                            communityCards.Remove(hand[i]);
                        }
                    }
                }

                return result;
            }
        }
    }
}