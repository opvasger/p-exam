using System;
using System.Collections.Generic;
using System.IO;
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
            Console.WriteLine(sets[0].name);
        })
        .Wait();
    }
}
