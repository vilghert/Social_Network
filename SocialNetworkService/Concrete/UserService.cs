using MongoDB.Bson;

public class UserService : IUserService
{
    private readonly IUserDal _mongoUserDal;
    private readonly INeo4jUserDal _neo4JUserDal;
    private readonly IRelationshipDal _relationshipDal;

    public UserService(IUserDal mongoUserDal, INeo4jUserDal neo4JUserDal, IRelationshipDal relationshipDal)
    {
        _mongoUserDal = mongoUserDal;
        _neo4JUserDal = neo4JUserDal;
        _relationshipDal = relationshipDal;
    }

    public async Task AddUserAsync(UserDto user)
    {
        await _mongoUserDal.AddUserAsync(user);
        string firstName = user.FirstName ?? "Unnamed";
        await _neo4JUserDal.CreateUserAsync(user.Id.ToString(), firstName);
    }

    public async Task DeleteUserAsync(string userId)
    {
        if (ObjectId.TryParse(userId, out ObjectId objectId))
        {
            await _mongoUserDal.DeleteUserAsync(objectId);
            await _neo4JUserDal.DeleteUserAsync(userId);
        }
        else
        {
            throw new ArgumentException("Invalid userId format.", nameof(userId));
        }
    }

    public async Task AddFriendshipAsync(ObjectId userId1, ObjectId userId2)
    {
        var friendship = new RelationshipDto
        {
            UserId1 = userId1,
            UserId2 = userId2,
            RelationshipType = "FRIEND",
            CreatedAt = DateTime.UtcNow
        };

        await _relationshipDal.AddRelationshipAsync(friendship);
        await _neo4JUserDal.CreateFriendAsync(userId1.ToString(), userId2.ToString());
    }

    public async Task RemoveFriendshipAsync(ObjectId friendshipId)
    {
        var friendship = await _relationshipDal.GetRelationshipByIdAsync(friendshipId);
        if (friendship != null)
        {
            await _relationshipDal.DeleteRelationshipAsync(friendship.UserId1, friendship.UserId2, "FRIEND");
            await _neo4JUserDal.DeleteFriendAsync(friendship.UserId1.ToString(), friendship.UserId2.ToString());
        }
    }

    public async Task FollowUserAsync(ObjectId followerId, ObjectId followeeId)
    {
        await _neo4JUserDal.CreateFollowerAsync(followerId.ToString(), followeeId.ToString());
    }

    public async Task UnfollowUserAsync(ObjectId followerId, ObjectId followeeId)
    {
        await _neo4JUserDal.DeleteFollowerAsync(followerId.ToString(), followeeId.ToString());
    }

    public async Task SubscribeUserAsync(ObjectId subscriberId, ObjectId subscribedId)
    {
        await _neo4JUserDal.CreateSubscriberAsync(subscriberId.ToString(), subscribedId.ToString());
    }

    public async Task UnsubscribeUserAsync(ObjectId subscriberId, ObjectId subscribedId)
    {
        await _neo4JUserDal.DeleteSubscriberAsync(subscriberId.ToString(), subscribedId.ToString());
    }
}
