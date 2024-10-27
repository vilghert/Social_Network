using MongoDB.Bson;
using System;
using System.Threading.Tasks;

public class PostViewModel
{
    private readonly PostDal _postDal;

    public PostViewModel(PostDal postDal)
    {
        _postDal = postDal;
    }

    public async Task AddPostAsync(string content, ObjectId userId)
    {
        var newPost = new PostDto
        {
            userId = userId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        await _postDal.InsertPostAsync(newPost);
        
    }
}
