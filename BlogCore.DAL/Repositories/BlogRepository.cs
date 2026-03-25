namespace BlogCore.DAL.Repositories;

using BlogCore.DAL.Data;
using BlogCore.DAL.Models;

public class BlogRepository
{
    private readonly BlogContext _context;

    public BlogRepository(BlogContext context)
    {
        _context = context;
    }

    public void AddPost(Post post)
    {
        _context.Posts.Add(post);
        _context.SaveChanges();
    }

    public IEnumerable<Post> GetAllPosts()
    {
        return [.. _context.Posts];
    }

    public void AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        _context.SaveChanges();
    }

    public IEnumerable<Comment> GetCommentsByPostId(int postId)
    {
        return [.. _context.Comments.Where(c => c.PostId == postId)];
    }

    public void DeletePost(int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.Id == postId);

        if (post is null)
        {
            return;
        }

        _context.Posts.Remove(post);
        //_context.SaveChanges();
    }

}
