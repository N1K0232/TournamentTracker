using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface ITeamService
{
    Task<Result<Team>> CreateAsync(SaveTeamRequest request, CancellationToken cancellationToken);

    Task<Result<Team>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Team>>> GetListAsync(CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveTeamRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}