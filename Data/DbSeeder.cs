using Education.Api.Models;
using Education.Api.Services.Abstractions.Curriculums;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Data;

public class DbSeeder(ApplicationDbContext context, ICurriculumService curriculumService)
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICurriculumService _curriculumService = curriculumService;

    /// <summary>
    /// Seeds curriculum data into the database by deserializing from a file
    /// and adding any curriculums that do not already exist in the database.
    /// </summary>
    public async Task SeedCurriculums()
    {
        // Seeds curriculums into the database
        List<Curriculum> curriculums =
            await _curriculumService.DeserializeCurriculumsFromFileAsync();

        foreach (var curriculum in curriculums)
        {
            if (!await _context.Curriculums.AnyAsync(x => x.Name == curriculum.Name))
            {
                await _context.Curriculums.AddAsync(curriculum);
            }
        }

        await _context.SaveChangesAsync();
    }
}
