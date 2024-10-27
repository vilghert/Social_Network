using MongoDB.Bson;

namespace Social_Network.DAL.Interface
{
    public interface ICommentDal
    {
        Task AddCommentAsync(CommentDto comment);
        Task<List<CommentDto>> GetCommentsByPostIdAsync(ObjectId postId);
        Task DeleteCommentAsync(ObjectId commentId);
    }
}