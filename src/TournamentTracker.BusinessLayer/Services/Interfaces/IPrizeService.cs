using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface IPrizeService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Prize>> GetAsync(Guid id);

    Task<Result<IEnumerable<Prize>>> GetListAsync();

    Task<Result<Prize>> CreateAsync(SavePrizeRequest request);

    Task<Result<Prize>> UpdateAsync(Guid id, SavePrizeRequest request);
}