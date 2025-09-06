using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Tags;

public interface ITagService
{
    Task<Tag> GetByNameAsync(string name);

    Task UpdateQuestionTagsAsync(int questionId, List<string> newTagNames);
}
