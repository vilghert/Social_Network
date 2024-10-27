using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

public class RelationshipDto
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ObjectId UserId1 { get; set; }
    public ObjectId UserId2 { get; set; }

    public string? RelationshipType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}