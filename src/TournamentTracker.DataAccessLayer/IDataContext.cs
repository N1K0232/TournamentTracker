using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer;

public interface IDataContext
{
    Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity;

    Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity;

    Task DeleteAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity;

    ValueTask<TEntity?> GetAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : BaseEntity;

    IQueryable<TEntity> GetData<TEntity>(bool trackingChanges = false) where TEntity : BaseEntity;

    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    Task ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);

    Task<T> ExecuteTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}