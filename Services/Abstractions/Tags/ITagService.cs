using Education.Api.Models;

namespace Education.Api.Services.Abstractions.Tags;

public interface ITagService
{
    Task<Tag> GetByName(string name);
}
