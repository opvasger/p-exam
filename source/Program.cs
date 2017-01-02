using System;
using System.Collections.Generic;
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

            // Log the name of the set with most legendary cards
            var mostLegendarySet = sets.Select(set => new {
                SetName = set.Name,
                LegendCount = set.Cards
                    .Where(card =>
                        card.SuperTypes != null
                        &&  card.SuperTypes
                            .Contains("Legendary"))
                            .ToList()
                            .Count
            })
            .OrderByDescending(set => set.LegendCount)
            .FirstOrDefault();

            Console.WriteLine(
                "{0} is the most legendary set with {1} legendary cards",
                mostLegendarySet.SetName,
                mostLegendarySet.LegendCount
            );

            // Log the count of all red cards
            var redCards = sets
            .SelectMany(set => set.Cards)
            .Where(card => card.Colors != null && card.Colors.Contains("R"))
            .ToList()

            Console.WriteLine(
                "There is {0} red Magic cards",
                redCards.Count
            );

            // Log the name of the most reprinted card
            var mostReprinted = sets
            .SelectMany(set => set.Cards)
            .Aggregate(
                new Dictionary<string, int>(),
                (cardMap, card) => {
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


            // Log the most popular combination of colors
            var mostPopularColorCombination =
            sets
            .SelectMany(set => set.Cards)
            .Where(card => card.Colors != null && card.Colors.Count > 1)
            .Aggregate(
                new Dictionary<string, int>(),
                (cardMap, card) => {
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

        })
        .Wait();
    }
}
