using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

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
            PrintReprinted(sets);
            PrintLegendary(sets);
            PrintRed(sets);

            watch.Stop();
            var syncElapsed = watch.Elapsed.TotalMilliseconds;
            watch.Reset();

            Console.WriteLine("\nElapsed Time running synchronously was {0} miliseconds\n", syncElapsed);
            

            watch.Start();

            PrintColorParallel(sets);
            //PrintReprintParallel(sets);
            //PrintLegendaryParallel(sets);
            //PrintRedParallel(sets);

            watch.Stop();
            var parallelElapsed = watch.Elapsed.TotalMilliseconds;
            watch.Reset();

            Console.WriteLine("\nElapsed Time in parallel was {0} miliseconds\n", parallelElapsed);
            

        })
        .Wait();
    }

    public static void PrintColor(List<Model.Set> sets)
    {
        var mostPopularColorCombination =
            sets
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

    public static void PrintColorParallel(List<Model.Set> sets)
    {
        var mostPopularColorCombination =
            sets.AsParallel()
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

    public static void PrintReprinted(List<Model.Set> sets)
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

    public static void PrintLegendary(List<Model.Set> sets)
    {
        var mostLegendarySet = sets.Select(set => new
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

    public static void PrintRed(List<Model.Set> sets)
    {
        var redCards = sets
            .SelectMany(set => set.Cards)
            .Where(card => card.Colors != null && card.Colors.Contains("R"))
            .ToList();

        Console.WriteLine(
            "There is {0} red Magic cards",
            redCards.Count
        );
    }
}
