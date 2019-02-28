﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayingCards
{
    public static partial class Poker
    {
        public static class EquityCalculator
        {
            public class Result
            {
                public float Total;
                public float Win;
                public float Split;
            }

            public static async Task<List<Result>> Calc(List<List<Default.Card>> hands, List<Default.Card> communityCards)
            {
                var result = new List<Result>();

                var deck = Deck;
                var allCards = hands.SelectMany(x => x);
                deck.RemoveAll(x => allCards.Contains(x) || communityCards.Contains(x));
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
                        equ += splitCounters[i, j] / j;
                    }
                    equ = equ * 100 / times + win;
                    result.Add(new Result { Total = (float)Math.Round(equ, 2), Win = (float)Math.Round(win, 2), Split = (float)Math.Round(split, 2) });
                }

                return result;
            }

            private static Task<(int[], int[,])> CreateWorker(List<List<Default.Card>> hands, List<Default.Card> communityCards, List<Default.Card> deck, BlockingCollection<int> indexes)
            {
                return Task.Run(() =>
                {
                    var comm = new List<Default.Card>(communityCards);
                    var restCards = 5 - comm.Count;
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
                            comm.Add(deck[deckIndexes & 0x3f]);
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

                        for (var i = 0; i < restCards; i++)
                        {
                            comm.RemoveAt(comm.Count - 1);
                        }
                    };

                    return (counters, splitCounters);
                });
            }

            private static int GetWinners(List<List<Default.Card>> hands, List<Default.Card> communityCards)
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

            private static List<ulong> GetHandCodes(List<List<Default.Card>> hands, List<Default.Card> communityCards)
            {
                var communityCardSize = communityCards.Count;
                communityCards.Add(hands[0][0]);
                communityCards.Add(hands[0][1]);
                var result = new List<ulong>();

                foreach (var hand in hands)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        communityCards[communityCardSize + i] = hand[i];
                    }
                    result.Add(Hand.Encode(communityCards));
                }

                communityCards.RemoveRange(communityCardSize, 2);

                return result;
            }

        }
    }
}