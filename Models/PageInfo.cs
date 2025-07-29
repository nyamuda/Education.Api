namespace Education.Api.Models;

public class PageInfo<T>
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }

    public required bool HasMore { get; set; }

    public required List<T> Items { get; set; }
}
