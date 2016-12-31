using System;
using System.Net.Http;

public class Program
{
    public static void Main (string[] args)
    {
        new HttpClient()
        .GetAsync("http://mtgjson.com/json/AllSetsArray-x.json")
        .ContinueWith(async response => {
            var result = await response;
            var body = await result.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        })
        .Wait();
    }
}
