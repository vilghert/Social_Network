using MongoDB.Bson;

public interface IUserDal
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(ObjectId userId);
    Task AddUserAsync(UserDto user);
    Task UpdateUserAsync(UserDto user);
    Task DeleteUserAsync(ObjectId userId);
    Task AddFriendAsync(ObjectId userId, ObjectId friendId);
    Task RemoveFriendAsync(ObjectId userId, ObjectId friendId);
    Task<UserDto> LoginAsync(string email, string password);
}