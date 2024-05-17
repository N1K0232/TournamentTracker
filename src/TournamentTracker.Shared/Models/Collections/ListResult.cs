namespace TournamentTracker.Shared.Models.Collections;

public class ListResult<T>
{
    public IEnumerable<T>? Content { get; init; }

    public long TotalCount { get; init; }

    public bool HasNextPage { get; init; }
}