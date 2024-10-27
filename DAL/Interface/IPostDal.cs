using MongoDB.Bson;

public interface IPostDal
{
    Task<List<PostDto>> GetAllPostsAsync();
    Task<PostDto> GetPostByIdAsync(ObjectId postId);
    Task AddPostAsync(PostDto post);
    Task UpdatePostAsync(PostDto post);
    Task DeletePostAsync(ObjectId postId);
}
