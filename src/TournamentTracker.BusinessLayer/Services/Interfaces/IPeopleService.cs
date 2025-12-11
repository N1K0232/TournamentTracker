using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<Result<Person>> CreateAsync(SavePersonRequest request, CancellationToken cancellationToken);

    Task<Result<Person>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Person>>> GetListAsync(CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SavePersonRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}