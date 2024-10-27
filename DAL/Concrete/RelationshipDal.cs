using MongoDB.Bson;
using MongoDB.Driver;
public class RelationshipDal : IRelationshipDal
{
    private readonly IMongoCollection<RelationshipDto> _relationships;

    public RelationshipDal(IMongoDatabase database)
    {
        _relationships = database.GetCollection<RelationshipDto>("relationships");
    }

    public async Task AddRelationshipAsync(RelationshipDto relationship)
    {
        await _relationships.InsertOneAsync(relationship);
    }

    public async Task<List<RelationshipDto>> GetRelationshipsByUserIdAsync(ObjectId userId, string relationshipType)
    {
        return await _relationships.Find(r =>
            (r.UserId1 == userId || r.UserId2 == userId) &&
            r.RelationshipType == relationshipType).ToListAsync();
    }

    public async Task DeleteRelationshipAsync(ObjectId userId1, ObjectId userId2, string relationshipType)
    {
        await _relationships.DeleteOneAsync(r =>
            ((r.UserId1 == userId1 && r.UserId2 == userId2) ||
             (r.UserId1 == userId2 && r.UserId2 == userId1)) &&
            r.RelationshipType == relationshipType);
    }

    public async Task<RelationshipDto> GetRelationshipAsync(ObjectId userId1, ObjectId userId2, string relationshipType)
    {
        return await _relationships.Find(r =>
            ((r.UserId1 == userId1 && r.UserId2 == userId2) ||
             (r.UserId1 == userId2 && r.UserId2 == userId1)) &&
            r.RelationshipType == relationshipType).FirstOrDefaultAsync();
    }

    public async Task<RelationshipDto> GetRelationshipByIdAsync(ObjectId relationshipId)
    {
        return await _relationships.Find(r => r.Id == relationshipId).FirstOrDefaultAsync();
    }
    public async Task InsertAsync(RelationshipDto relationship)
    {
        await _relationships.InsertOneAsync(relationship);
    }
}
