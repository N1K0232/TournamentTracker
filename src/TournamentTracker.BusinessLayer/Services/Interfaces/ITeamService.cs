using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface ITeamService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Team>> GetAsync(Guid id);

    Task<Result<PaginatedList<Team>>> GetListAsync(string name, int pageIndex, int itemsPerPage);

    Task<Result<Team>> InsertAsync(SaveTeamRequest request);

    Task<Result<Team>> UpdateAsync(Guid id, SaveTeamRequest request);
}