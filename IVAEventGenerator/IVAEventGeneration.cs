using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace IVAEventGerator;
public static class IVAEventGeneration
{
    public static async Task GenerateAndForwardAsync(
        IMongoCollection<IVAEvents> collection,
        ILogstashForwarderService forwarder,
        ObjectId cameraId,
        int count,
        DateTime startTime,
        TimeSpan interval)
    {
        Random random = new((int)DateTime.Now.Ticks);

        for (int i = 0; i < count; i++)
        {
            var objTOAdd = await GenerateEventAsync(random, cameraId, startTime, interval, i);
            await collection.InsertOneAsync(objTOAdd);
            await forwarder.ForwardEventAsync(objTOAdd);
        }
        Console.WriteLine($"Event Generated and Forwarded to Logstash {count}");
    }
    private static async Task<IVAEvents> GenerateEventAsync(
        Random random,
        ObjectId cameraId,
        DateTime startTime,
        TimeSpan interval,
        int index)
    {
        int nDetection = random.Next(0, 16);
        nDetection = nDetection < 6 ? 0 : nDetection - 5;

        var ivaEvent = new IVAEvents
        {
            Timestamp = startTime + interval * index,
            Metadata = new(cameraId),
            ConfidenceScore = random.NextDouble()
        };

        if (nDetection > 0)
        {
            Rectangle[] rectangles = new Rectangle[nDetection];
            for (int j = 0; j < nDetection; j++)
            {
                rectangles[j] = new(
                    random.Next(0, 1821),
                    random.Next(0, 981),
                    random.Next(30, 101),
                    random.Next(40, 101)
                );
            }
            ivaEvent.Rectangles = rectangles;
        }

        return ivaEvent;
    }
}