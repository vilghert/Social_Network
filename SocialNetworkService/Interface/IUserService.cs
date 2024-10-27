using MongoDB.Bson;

public interface IUserService
{
    Task AddUserAsync(UserDto user);
    Task DeleteUserAsync(string userId);
    Task AddFriendshipAsync(ObjectId userId1, ObjectId userId2);
    Task RemoveFriendshipAsync(ObjectId friendshipId);
    Task FollowUserAsync(ObjectId followerId, ObjectId followeeId);
    Task UnfollowUserAsync(ObjectId followerId, ObjectId followeeId);
    Task SubscribeUserAsync(ObjectId subscriberId, ObjectId subscribedId);
    Task UnsubscribeUserAsync(ObjectId subscriberId, ObjectId subscribedId);
}