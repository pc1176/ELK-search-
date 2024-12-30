using MongoDB.Bson;

namespace IVAEventGerator;
public class IVAEvents
{
    public ObjectId Id { get; set; }
    public string EventId { get; set; }
    public DateTime Timestamp { get; set; }
    public Metadata Metadata { get; set; }
    public Rectangle[]? Rectangles { get; set; }
    public double ConfidenceScore { get; set; }

    public IVAEvents()
    {
        EventId = $"{Guid.NewGuid()}";
    }
}

public class Rectangle(int x, int y, int width, int height)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
    public int Width { get; set; } = width;
    public int Height { get; set; } = height;
}

public class Metadata(ObjectId cameraID)
{
    public ObjectId CameraId { get; set; } = cameraID;
}