using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TournamentTracker.DataAccessLayer.Entities.Common;
using TournamentTracker.DataAccessLayer.Extensions;

namespace TournamentTracker.DataAccessLayer;

public class DataContext : DbContext, IDataContext
{
    private static readonly MethodInfo setQueryFilterOnDeletableEntity = typeof(DataContext)
        .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
        .Single(e => e.IsGenericMethod && e.Name == nameof(SetQueryFilterOnDeletableEntity));

    private readonly ILogger<DataContext> logger;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public DataContext(DbContextOptions<DataContext> options, ILogger<DataContext> logger) : base(options)
    {
        this.logger = logger;
    }

    public void Delete<T>(T entity) where T : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        Set<T>().Remove(entity);
    }

    public void Delete<T>(IEnumerable<T> entities) where T : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entities, nameof(entities));
        Set<T>().RemoveRange(entities);
    }

    public async ValueTask<T> GetAsync<T>(Guid id) where T : BaseEntity
    {
        var entity = await Set<T>().FindAsync([id], tokenSource.Token);
        return entity;
    }

    public IQueryable<T> GetData<T>(bool ignoreQueryFilters = false, bool trackingChanges = false) where T : BaseEntity
    {
        var set = Set<T>().IgnoreQueryFilters(ignoreQueryFilters).TrackChanges(trackingChanges);
        return set;
    }

    public void Insert<T>(T entity) where T : BaseEntity
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        Set<T>().Add(entity);
    }

    public async Task SaveAsync()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.Entity.GetType()))
            .ToList();

        foreach (var entry in entries.Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            var entity = entry.Entity as BaseEntity;

            if (entry.State is EntityState.Modified)
            {
                if (entity is DeletableEntity deletableEntity)
                {
                    deletableEntity.IsDeleted = false;
                    deletableEntity.DeletedDate = null;
                }

                entity.LastModificationDate = DateTime.UtcNow;
            }

            if (entry.State is EntityState.Deleted)
            {
                if (entity is DeletableEntity deletableEntity)
                {
                    deletableEntity.IsDeleted = true;
                    deletableEntity.DeletedDate = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }
        }

        var affectedRows = await SaveChangesAsync(true, tokenSource.Token);
        logger.LogInformation("Rows affected: {affectedRows}", affectedRows);
    }

    public async Task ExecuteTransactionAsync(Func<Task> action)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await Database.BeginTransactionAsync(tokenSource.Token);
            await action.Invoke();

            await transaction.CommitAsync(tokenSource.Token);
            await transaction.DisposeAsync();
        });
    }

    public override void Dispose()
    {
        if (tokenSource is not null)
        {
            tokenSource.Dispose();
            tokenSource = null;
        }

        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        OnModelCreatingCore(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void OnModelCreatingCore(ModelBuilder modelBuilder)
    {
        var entityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType))
            .ToList();

        foreach (var type in entityTypes.Select(t => t.ClrType))
        {
            var methods = SetGlobalQueryFiltersMethod(type);
            foreach (var method in methods)
            {
                method.MakeGenericMethod(type).Invoke(this, [modelBuilder]);
            }
        }
    }

    private static IEnumerable<MethodInfo> SetGlobalQueryFiltersMethod(Type type)
    {
        var methods = new List<MethodInfo>();

        if (typeof(DeletableEntity).IsAssignableFrom(type))
        {
            methods.Add(setQueryFilterOnDeletableEntity);
        }

        return methods;
    }

    private void SetQueryFilterOnDeletableEntity<T>(ModelBuilder modelBuilder) where T : DeletableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted && x.DeletedDate == null);
    }
}