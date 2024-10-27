using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ReactionDto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("reactionType")]
    public string? Type { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}