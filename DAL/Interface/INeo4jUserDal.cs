public interface INeo4jUserDal
{
    Task CreateUserAsync(string userId, string name);
    Task DeleteUserAsync(string userId);
    Task CreateRelationshipAsync(string userId1, string userId2, string relationshipType);
    Task DeleteRelationshipAsync(string userId1, string userId2, string relationshipType);
    Task CreateFriendAsync(string userId1, string userId2);
    Task DeleteFriendAsync(string userId1, string userId2);
    Task CreateFollowerAsync(string userId1, string userId2);
    Task DeleteFollowerAsync(string userId1, string userId2);
    Task CreateSubscriberAsync(string userId1, string userId2);
    Task DeleteSubscriberAsync(string userId1, string userId2);
    Task UpdateUserNameAsync(string userId, string newUserName);
    Task UpdateUserAsync(string userId, string newFirstName, string newEmail);
    Task<bool> AreUsersConnectedAsync(string userId1, string userId2);
    Task<int> GetDistanceBetweenUsersAsync(string userId1, string userId2);
}