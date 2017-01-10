using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {

        new HttpClient()
        .GetAsync("http://mtgjson.com/json/AllSetsArray-x.json")
        .ContinueWith(async responseTask =>
        {
            var response = await responseTask;
            var body = await response.Content.ReadAsStringAsync();
            var setSerializer = new DataContractJsonSerializer(typeof(List<Model.Set>));
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var sets = (List<Model.Set>)setSerializer.ReadObject(bodyStream);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            PrintColor(sets);
            PrintReprint(sets);
            PrintLegendary(sets);
            PrintRed(sets);

            PrintResult("\nElapsed Time running synchronously was \n{0} miliseconds\n", watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            PrintColorParallel(sets);
            PrintReprintParallel(sets);
            PrintLegendaryParallel(sets);
            PrintRedParallel(sets);

            PrintResult("\nElapsed Time with PLINQ was \n{0} miliseconds\n", watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            Task.WaitAll(new Task[] {
                            Task.Run(() => PrintColor(sets)),
                            Task.Run(() => PrintReprint(sets)),
                            Task.Run(() => PrintLegendary(sets)),
                            Task.Run(() => PrintRed(sets))
                        });

            PrintResult("\nElapsed Time in parallel was \n{0} miliseconds\n", watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            Task.WaitAll(new Task[] {
                            Task.Run(() => PrintColorParallel(sets)),
                            Task.Run(() => PrintReprintParallel(sets)),
                            Task.Run(() => PrintLegendaryParallel(sets)),
                            Task.Run(() => PrintRedParallel(sets))
                        });

            PrintResult("\nElapsed Time in parallel with PLINQ was \n{0} miliseconds\n", watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            Task.WaitAny(new Task[] {
                            Task.Run(() => PrintColor(sets)),
                            Task.Run(() => PrintReprint(sets)),
                            Task.Run(() => PrintLegendary(sets)),
                            Task.Run(() => PrintRed(sets))
                        });

            PrintResult("\nElapsed Time for the shortest task was \n{0} miliseconds\n", watch.Elapsed.TotalMilliseconds);
            watch.Stop();

        }).Wait();
    }

    private static void PrintResult(string message, double result)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message, result);
        Console.ResetColor();
    }

    // LINQ
    public static void PrintColor(List<Model.Set> sets)
    {
        var mostPopularColorCombination = sets
                .SelectMany(set => set.Cards)
                .Where(card => card.Colors != null && card.Colors.Count > 1)
                .Aggregate(
                    new Dictionary<string, int>(),
                    (cardMap, card) =>
                    {
                        var key = String.Join(",", card.Colors);

                        if (cardMap.ContainsKey(key))
                            cardMap[key] += 1;
                        else
                            cardMap.Add(key, 1);
                        return cardMap;
                    }
                )
                .OrderBy(card => card.Value)
                .LastOrDefault();

        Console.WriteLine(
            "{0} is the most popular Magic card color combination with {1} cards",
            mostPopularColorCombination.Key,
            mostPopularColorCombination.Value
        );
    }

    // PLINQ
    public static void PrintColorParallel(List<Model.Set> sets)
    {
        var mostPopularColorCombination = sets.AsParallel()
                .SelectMany(set => set.Cards)
                .Where(card => card.Colors != null && card.Colors.Count > 1)
                .Aggregate(
                    new Dictionary<string, int>(),
                    (cardMap, card) =>
                    {
                        var key = String.Join(",", card.Colors);

                        if (cardMap.ContainsKey(key))
                            cardMap[key] += 1;
                        else
                            cardMap.Add(key, 1);
                        return cardMap;
                    }
                )
                .OrderBy(card => card.Value)
                .LastOrDefault();

        Console.WriteLine(
            "{0} is the most popular Magic card color combination with {1} cards",
            mostPopularColorCombination.Key,
            mostPopularColorCombination.Value
        );
    }

    // LINQ
    public static void PrintReprint(List<Model.Set> sets)
    {
        var mostReprinted = sets
            .SelectMany(set => set.Cards)
            .Aggregate(
                new Dictionary<string, int>(),
                (cardMap, card) =>
                {
                    if (cardMap.ContainsKey(card.Name))
                        cardMap[card.Name] = cardMap[card.Name] + 1;
                    else
                        cardMap.Add(card.Name, 0);
                    return cardMap;
                }
            )
            .OrderByDescending(card => card.Value)
            .FirstOrDefault();

        Console.WriteLine(
            "{0} is the most reprinted Magic card with {1} reprints",
            mostReprinted.Key,
            mostReprinted.Value
        );
    }

    // PLINQ
    public static void PrintReprintParallel(List<Model.Set> sets)
    {
        var mostReprinted = sets.AsParallel()
            .SelectMany(set => set.Cards)
            .Aggregate(
                new Dictionary<string, int>(),
                (cardMap, card) =>
                {
                    if (cardMap.ContainsKey(card.Name))
                        cardMap[card.Name] = cardMap[card.Name] + 1;
                    else
                        cardMap.Add(card.Name, 0);
                    return cardMap;
                }
            )
            .OrderByDescending(card => card.Value)
            .FirstOrDefault();

        Console.WriteLine(
            "{0} is the most reprinted Magic card with {1} reprints",
            mostReprinted.Key,
            mostReprinted.Value
        );
    }

    // LINQ
    public static void PrintLegendary(List<Model.Set> sets)
    {
        var mostLegendarySet = sets
            .Select(set => new
            {
                SetName = set.Name,
                LegendCount = set.Cards
                    .Where(card =>
                        card.SuperTypes != null
                        && card.SuperTypes
                            .Contains("Legendary"))
                    .ToList()
                    .Count
            })
            .OrderByDescending(set => set.LegendCount)
            .FirstOrDefault();

        Console.WriteLine(
            "{0} are the most legendary set with {1} legendary cards",
            mostLegendarySet.SetName,
            mostLegendarySet.LegendCount
        );
    }

    // PLINQ
    public static void PrintLegendaryParallel(List<Model.Set> sets)
    {
        var mostLegendarySet = sets.AsParallel()
            .Select(set => new
            {
                SetName = set.Name,
                LegendCount = set.Cards
                    .Where(card =>
                        card.SuperTypes != null
                        && card.SuperTypes
                            .Contains("Legendary"))
                    .ToList()
                    .Count
            })
            .OrderByDescending(set => set.LegendCount)
            .FirstOrDefault();

        Console.WriteLine(
            "{0} are the most legendary set with {1} legendary cards",
            mostLegendarySet.SetName,
            mostLegendarySet.LegendCount
        );
    }

    // Synchronous Loop
    public static void PrintRed(List<Model.Set> sets)
    {
        var count = 0;

        for (var i = 0; i < sets.Count; i++)
        {
            for (var j = 0; j < sets[i].Cards.Count; j++)
            {
                if (sets[i].Cards[j].Colors != null)
                {
                    for (var k = 0; k < sets[i].Cards[j].Colors.Count; k++)
                    {
                        if (sets[i].Cards[j].Colors[k] == "R")
                        {
                            count++;
                            break;
                        }
                    }
                }
            }
        }

        Console.WriteLine(
            "There is {0} red Magic cards",
            count
        );
    }

    // Asynchronous Loop
    public static void PrintRedParallel(List<Model.Set> sets)
    {
        var count = 0;

        Parallel.For(0, sets.Count, i =>
        {
            Parallel.ForEach(Partitioner.Create(0, sets[i].Cards.Count),
                (range) =>
                {
                    for (int j = range.Item1; j < range.Item2; j++)
                    {
                        if (sets[i].Cards[j].Colors != null)
                        {
                            for (var k = 0; k < sets[i].Cards[j].Colors.Count; k++)
                            {
                                if (sets[i].Cards[j].Colors[k] == "R")
                                {
                                    Interlocked.Increment(ref count);
                                    break;
                                }
                            }
                        }
                    }
                });
        });

        Console.WriteLine(
            "There is {0} red Magic cards",
            count
        );
    }
}
