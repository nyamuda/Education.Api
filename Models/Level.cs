namespace Education.Api.Models;

public class Level
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int ExamBoardId { get; set; }

    public ExamBoard? ExamBoard { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
