using Education.Api.Data;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Tags;
using Microsoft.EntityFrameworkCore;

namespace Education.Api.Services.Implementations.Tags;

public class TagService(ApplicationDbContext context, ILogger<TagService> logger) : ITagService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<TagService> _logger = logger;

    /// <summary>
    /// Retrieves a tag by its name.
    /// If no tag with the specified name exists, a new tag is created and added to the database.
    /// </summary>
    /// <param name="name">The name of the tag to retrieve or create.</param>
    /// <returns>The existing or newly created <see cref="Tag"/>.</returns>
    public async Task<Tag> GetByNameAsync(string name)
    {
        var tag = await _context
            .Tags
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Name.ToLower().Equals(name.ToLower()));

        if (tag is null)
        {
            tag = new Tag { Name = name };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "No existing tag found. Created new tag with name: {TagName}",
                tag.Name
            );
        }

        return tag;
    }
}
