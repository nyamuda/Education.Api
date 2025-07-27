using Education.Api.Enums;

namespace Education.Api.Models;

public class Question
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public int? Marks { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public List<Like> Likes { get; set; } = [];
    public List<Upvote> Upvotes { get; set; } = [];

    public List<PostFlagType> Flags { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
