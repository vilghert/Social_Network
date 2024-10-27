using MongoDB.Bson;
using MongoDB.Driver;
using Social_Network.DAL.Interface;

namespace Social_Network.DAL.Concrete
{
    public class CommentDal : ICommentDal
    {
        private readonly IMongoCollection<CommentDto> _comments;

        public CommentDal(IMongoDatabase database)
        {
            _comments = database.GetCollection<CommentDto>("comments");
        }

        public async Task AddCommentAsync(CommentDto comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByPostIdAsync(ObjectId postId)
        {
            return await _comments.Find(c => c.PostId == postId).ToListAsync();
        }

        public async Task DeleteCommentAsync(ObjectId commentId)
        {
            await _comments.DeleteOneAsync(c => c.Id == commentId);
        }
        private readonly IMongoCollection<CommentDto> _commentsCollection;

        public async Task<List<CommentDto>> GetAllCommentsAsync()
        {
            return await _commentsCollection.Find(_ => true).ToListAsync();
        }

        public async Task InsertCommentAsync(CommentDto comment)
        {
            await _commentsCollection.InsertOneAsync(comment);
        }
    }
}
