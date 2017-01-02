using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

/*
    Simple example of a HTTP get and body parsing to string.
    - build the program with dotnet restore.
    - run the program with dotnet watch run.
*/

public class Program
{
    public static void Main(string[] args)
    {

        new HttpClient()
        .GetAsync("http://mtgjson.com/json/AllSetsArray-x.json")
        .ContinueWith(async response =>
        {
            var result = await response;
            var body = await result.Content.ReadAsStringAsync();
            var setSerializer = new DataContractJsonSerializer(typeof(List<Model.Set>));
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var sets = (List<Model.Set>) setSerializer.ReadObject(bodyStream);

            // Log the name of the set with most legendary cards

            // Log the count of all red cards
            Console.WriteLine(
                "There is {0} red Magic cards",
                sets
                .SelectMany(set => set.Cards)
                .Where(card => card.Colors != null && card.Colors.Contains("R"))
                .ToList()
                .Count
            );

            // Log the name of the most reprinted card

            // Log the most popular combination of colors
            var cards = sets
                        .SelectMany(set => set.Cards)
                        .Where(card => card.Colors != null);


            foreach (Model.Card card in cards)
                Console.WriteLine(card.Colors[0]);
        })
        .Wait();
    }
}
