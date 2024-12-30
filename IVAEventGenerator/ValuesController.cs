using IVAEventGerator;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http.HttpResults;

namespace IVAEventGenerator
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        string uri = "mongodb://localhost:27017/";
        TimeSpan halfSecond = new(0, 0, 0, 0, 100);
        ILogstashForwarderService logstashForwarder;
        public ValuesController(ILogstashForwarderService _logstashForwarder)
        {
             logstashForwarder = _logstashForwarder;
        }
        [HttpGet("camera/{count}")]
        public async Task<ActionResult> GenerateEvents(int count)
        {
            
            var tempCount = count;
            try
            {
                var client = new MongoClient(uri);
                var databaseNames = client.ListDatabaseNames().ToList();
                var db = client.GetDatabase("IVAdatabase");
                var collection = db.GetCollection<IVAEvents>("IVAserverevents");

                ObjectId[] cameraIDs = [
                    new("6719e5ece17510bc801ca679"),

];

                Task[] generatorTasks = new Task[cameraIDs.Length];

                var sw = Stopwatch.StartNew();



                var tempId = cameraIDs[0];
                var tempStartDay = DateTime.Now;
                var tempInterval = halfSecond * (IsEven(0) ? 2 : 1);
                
                generatorTasks[0] = Task.Run(async () =>
                {
                    await IVAEventGeneration.GenerateAndForwardAsync(
                        collection,
                        logstashForwarder,
                        tempId,
                        Convert.ToInt32(tempCount),
                        tempStartDay,
                        tempInterval
                    );
                });


                await Task.WhenAll(generatorTasks);

                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds);

                static bool IsEven(int i) => i % 2 == 0;
                Console.WriteLine("Successfully connected to MongoDB. Databases: {0} ", string.Join(", ", databaseNames));

                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to MongoDB. Error: {0}", ex.Message);
            }
            return BadRequest();
        }
    }
}
