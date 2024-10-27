using MongoDB.Bson;
using System.Threading.Tasks;

public interface IRelationshipDal
{
    Task AddRelationshipAsync(RelationshipDto relationship);
    Task InsertAsync(RelationshipDto relationship);
    Task<List<RelationshipDto>> GetRelationshipsByUserIdAsync(ObjectId userId, string relationshipType);
    Task<RelationshipDto> GetRelationshipAsync(ObjectId userId1, ObjectId userId2, string relationshipType);
    Task<RelationshipDto> GetRelationshipByIdAsync(ObjectId relationshipId);
    Task DeleteRelationshipAsync(ObjectId userId1, ObjectId userId2, string relationshipType);
}