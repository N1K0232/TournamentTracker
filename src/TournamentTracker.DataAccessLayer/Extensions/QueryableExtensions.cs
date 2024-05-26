using Microsoft.EntityFrameworkCore;

namespace TournamentTracker.DataAccessLayer.Extensions;

public static class QueryableExtensions
{
    public static async Task<bool> HasNextPageAsync<T>(this IQueryable<T> source, int pageIndex, int itemsPerPage, CancellationToken cancellationToken = default) where T : class
    {
        var list = await source.Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1).ToListAsync(cancellationToken);
        return list.Count > itemsPerPage;
    }
}