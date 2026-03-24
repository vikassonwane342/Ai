namespace CarMarketplace.Api.Common;

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }
}
