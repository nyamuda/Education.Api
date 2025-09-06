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

    /// <summary>
    /// Updates the tags associated with a specific question.
    /// Ensures that after the update, the question's tags exactly match the provided list of tag names.
    /// </summary>
    /// <param name="questionId">The ID of the question to update.</param>
    /// <param name="newTagNames">A list of tag names that the question should have.</param>
    /// <exception cref="KeyNotFoundException">Thrown if the specified question does not exist.</exception>
    public async Task UpdateQuestionTagsAsync(int questionId, List<string> newTagNames)
    {
        // STEP 1: Fetch the question along with its existing tags
        var question =
            await _context
                .Questions
                .Include(q => q.Tags)
                .FirstOrDefaultAsync(q => q.Id == questionId)
            ?? throw new KeyNotFoundException(
                "Unable to update question tags: specified question not found."
            );

        // STEP 2: Remove tags that are no longer in the new list
        foreach (var tag in question.Tags.ToList())
        {
            if (!newTagNames.Any(t => t.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
            {
                question.Tags.Remove(tag);
            }
        }

        // STEP 3: Add new tags that don't already exist
        foreach (var tagName in newTagNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (question.Tags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
                continue;

            var tag = await GetByNameAsync(tagName);
            question.Tags.Add(tag);
        }

        // STEP 4: Persist changes to the database
        await _context.SaveChangesAsync();
    }
}
