namespace Education.Api.Models;

public class Subject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Curriculum> Curriculums { get; set; } = [];
}
