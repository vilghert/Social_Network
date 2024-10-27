using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CommentDto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("postId")]
    public ObjectId PostId { get; set; }

    [BsonElement("userId")]
    public ObjectId UserId { get; set; }

    [BsonElement("content")]
    public string? Content { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("reactions")]
    public List<ReactionDto> Reactions { get; set; } = new List<ReactionDto>();
    public string? Text { get; set; }
}
