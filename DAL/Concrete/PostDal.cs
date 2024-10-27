using MongoDB.Bson;
using MongoDB.Driver;

public class PostDal : IPostDal
{
    private readonly IMongoCollection<PostDto> _posts;

    public PostDal(IMongoDatabase database)
    {
        _posts = database.GetCollection<PostDto>("posts");
    }
    public async Task InsertPostAsync(PostDto post)
    {
        await _posts.InsertOneAsync(post);
    }

    public async Task<List<PostDto>> GetAllPostsAsync()
    {
        return await _posts.Find(_ => true).ToListAsync();
    }

    public async Task<PostDto> GetPostByIdAsync(ObjectId postId)
    {
        return await _posts.Find(p => p.Id == postId).FirstOrDefaultAsync();
    }

    public async Task AddPostAsync(PostDto post)
    {
        await _posts.InsertOneAsync(post);
    }

    public async Task UpdatePostAsync(PostDto post)
    {
        await _posts.ReplaceOneAsync(p => p.Id == post.Id, post);
    }

    public async Task DeletePostAsync(ObjectId postId)
    {
        await _posts.DeleteOneAsync(p => p.Id == postId);
    }
    public async Task LikePostAsync(ObjectId postId, ObjectId userId)
    {
        var post = await GetPostByIdAsync(postId);
        if (post != null)
        {
            if (!post.Reactions.Any(r => r.UserId == userId))
            {
                post.Reactions.Add(new ReactionDto { UserId = userId, Type = "Like" });
                await UpdatePostAsync(post);
            }
        }
    }
    public async Task AddCommentAsync(ObjectId postId, CommentDto comment)
    {
        comment.PostId = postId;
        var filter = Builders<PostDto>.Filter.Eq(p => p.Id, postId);
        var update = Builders<PostDto>.Update.Push(p => p.Comments, comment);
        await _posts.UpdateOneAsync(filter, update);
    }
}