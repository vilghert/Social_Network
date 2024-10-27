using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class PostDto
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("AuthorId")]
    public ObjectId AuthorId { get; set; }

    [BsonElement("userId")]
    public ObjectId userId { get; set; }

    [BsonElement("content")]
    public string? Content { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("comments")]
    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();

    [BsonElement("reactions")]
    public List<ReactionDto> Reactions { get; set; } = new List<ReactionDto>();
}
