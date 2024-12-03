using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services;

public interface ITournamentService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Tournament>> GetAsync(Guid id);

    Task<Result<PaginatedList<Tournament>>> GetListAsync(string name, int pageIndex, int itemsPerPage);

    Task<Result<Tournament>> InsertAsync(SaveTournamentRequest request);

    Task<Result<Tournament>> UpdateAsync(Guid id, SaveTournamentRequest request);
}