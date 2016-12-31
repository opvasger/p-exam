using System;
using System.Net.Http;

/*
    Simple example of a HTTP get and body parsing to string.
    - build the program with dotnet restore.
    - run the program with dotnet run.
*/

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
