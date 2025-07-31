namespace Education.Api.Models;

/// <summary>
/// Represents a paginated result set, including items and metadata for paging.
/// </summary>
/// <typeparam name="T">The type of items being paginated.</typeparam>
public class PageInfo<T>
{
    /// <summary>
    /// The current page number (1-based index).
    /// </summary>
    public required int Page { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public required int PageSize { get; set; }

    /// <summary>
    /// Indicates whether there are more items available beyond the current page.
    /// </summary>
    public required bool HasMore { get; set; }

    /// <summary>
    /// The total number of items across all pages, if known.
    /// </summary>
    public int? TotalItems { get; set; }

    /// <summary>
    /// The list of items returned for the current page.
    /// </summary>
    public required List<T> Items { get; set; }
}
