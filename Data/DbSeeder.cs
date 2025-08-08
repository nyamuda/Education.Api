using Microsoft.EntityFrameworkCore;

namespace Education.Api.Data;

public class DbSeeder(DbContextOptionsBuilder optionsBuilder)
{
    private readonly DbContextOptionsBuilder _optionsBuilder = optionsBuilder;

    public void SeedCurriculums() { }
}
