using Education.Api.Data;
using Education.Api.Dtos.Curriculums;
using Education.Api.Services.Abstractions.Curriculums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Education.Api.Services.Implementations.Curriculums;

public class CurriculumService(ApplicationDbContext context) : ICurriculumService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<CurriculumDto> GetByIdAsync(int id)
    {
        return await _context
                .Curriculums
                .Select(
                    c =>
                        new CurriculumDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CreatedAt = c.CreatedAt
                        }
                )
                .FirstOrDefaultAsync(c => c.Id.Equals(id))
            ?? throw new KeyNotFoundException($@"Curriculum with ID ""{id}"" does not exist.");
    }
}
