namespace Education.Api.Models;

// A simple POCO representing a Student
public class Student
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}


