using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface ITournamentService
{
    Task<Result<Tournament>> CreateAsync(SaveTournamentRequest request, CancellationToken cancellationToken);

    Task<Result<Tournament>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Tournament>>> GetListAsync(CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveTournamentRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}