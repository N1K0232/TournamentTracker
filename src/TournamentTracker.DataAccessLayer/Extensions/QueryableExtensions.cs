using Microsoft.EntityFrameworkCore;

namespace TournamentTracker.DataAccessLayer.Extensions;

public static class QueryableExtensions
{
    public static async Task<IList<T>> ToListAsync<T>(this IQueryable<T> source, int pageIndex, int itemsPerPage, CancellationToken cancellationToken = default) where T : class
    {
        var skip = pageIndex * itemsPerPage;
        var take = itemsPerPage + 1;

        return await source.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public static async Task<bool> HasNextPageAsync<T>(this IQueryable<T> source, int pageIndex, int itemsPerPage, CancellationToken cancellationToken = default) where T : class
    {
        var list = await source.Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1).ToListAsync(cancellationToken);
        return list.Count > itemsPerPage;
    }

    internal static IQueryable<T> IgnoreQueryFilters<T>(this IQueryable<T> source, bool ignoreQueryFilters) where T : class
    {
        return ignoreQueryFilters ? source.IgnoreQueryFilters() : source;
    }

    internal static IQueryable<T> TrackChanges<T>(this IQueryable<T> source, bool trackingChanges) where T : class
    {
        return trackingChanges ? source.AsTracking() : source.AsNoTrackingWithIdentityResolution();
    }
}