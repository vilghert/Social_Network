using MongoDB.Bson;
using MongoDB.Driver;

namespace SocialNetwork_App.DAL.Concrete
{
    public class UserDal : IUserDal
    {
        private readonly IMongoCollection<UserDto> _users;

        public UserDal(IMongoDatabase database)
        {
            _users = database.GetCollection<UserDto>("users");
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(ObjectId userId)
        {
            return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }

        public async Task AddUserAsync(UserDto user)
        {
            user.Id = ObjectId.GenerateNewId();
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateUserAsync(UserDto user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task DeleteUserAsync(ObjectId userId)
        {
            await _users.DeleteOneAsync(u => u.Id == userId);
        }

        public async Task AddFriendAsync(ObjectId userId, ObjectId friendId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null && !user.Friends.Contains(friendId))
            {
                user.Friends.Add(friendId);
                await UpdateUserAsync(user);
            }
        }

        public async Task RemoveFriendAsync(ObjectId userId, ObjectId friendId)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null && user.Friends.Contains(friendId))
            {
                user.Friends.Remove(friendId);
                await UpdateUserAsync(user);
            }
        }

        public async Task<UserDto> LoginAsync(string email, string password)
        {
            return await _users.Find(u => u.Email == email && u.Password == password).FirstOrDefaultAsync();
        }
        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}