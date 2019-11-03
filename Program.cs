namespace Writer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;

    class Program
    {
        private const string DatabaseString = "test";
        private const string CollectionString = "items";
        private static string[] colors = new[] { "red", "orange", "yellow", "green", "blue", "indigo", "violet" };
        private static string[] animals = new[] { "cat", "dog", "llama", "hedgehog", "fish" };

        static async Task Main(string[] args)
        {
            string region = args[0];
            string connectionString = File.ReadAllText("CONNSTRING");

            MongoClient mongoClient = new MongoClient($"{connectionString}{region}");
            IMongoDatabase database = mongoClient.GetDatabase(Program.DatabaseString);
            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(Program.CollectionString);

            Random random = new Random();
            Stopwatch stopwatch = new Stopwatch();

            Guid session = Guid.NewGuid();
            int index = 0;
            while (true)
            {
                stopwatch.Restart();
                await collection.InsertOneAsync(Program.GetDocument(random, region, session, index++));
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }

        private static BsonDocument GetDocument(Random random, string region, Guid session, int index)
        {
            string color = Program.colors[(int)Math.Log10(random.Next(1, (int)Math.Pow(10, Program.colors.Length)))];
            string animal = Program.animals[(int)Math.Log10(random.Next(1, (int)Math.Pow(10, Program.animals.Length)))];
            return BsonDocument.Parse($"{{session:\"{session}\",index:{index},region:\"{region}\",color:\"{color}\",animal:\"{animal}\",a:{random.Next(1, 10)},b:{random.Next(1, 100)},c:{random.Next(1, 1000)},d:{random.Next(1, 10000)},e:{random.Next(1, 100000)}}}");
        }
    }
}
