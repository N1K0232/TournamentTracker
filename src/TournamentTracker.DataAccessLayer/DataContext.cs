using System.Reflection;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using TournamentTracker.DataAccessLayer.Entities.Common;

namespace TournamentTracker.DataAccessLayer;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IDataContext
{
    public async Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        await Set<TEntity>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        Set<TEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async ValueTask<TEntity?> GetAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : BaseEntity
    {
        var entity = await Set<TEntity>().FindAsync([id], cancellationToken).ConfigureAwait(false);
        return entity;
    }

    public IQueryable<TEntity> GetData<TEntity>(bool trackingChanges = false) where TEntity : BaseEntity
    {
        var set = Set<TEntity>();
        return trackingChanges ? set.AsTracking() : set.AsNoTrackingWithIdentityResolution();
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.Entity.GetType()))
            .ToList();

        foreach (var entry in entries.Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State is EntityState.Modified)
            {
                entity.LastModifiedAt = DateTime.UtcNow;
            }
        }

        return await SaveChangesAsync(true, cancellationToken).ConfigureAwait(false);
    }

    public async Task ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async (token) => await ExecuteTransactionInternalAsync(action, token).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> ExecuteTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async (token) => await ExecuteTransactionInternalAsync(action, token), cancellationToken).ConfigureAwait(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    private async Task ExecuteTransactionInternalAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            await action.Invoke(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    private async Task<T> ExecuteTransactionInternalAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        await using var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await action.Invoke(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}