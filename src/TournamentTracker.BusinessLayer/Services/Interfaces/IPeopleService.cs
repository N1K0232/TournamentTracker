using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Person>> GetAsync(Guid id);

    Task<Result<ListResult<Person>>> GetListAsync(string name, string orderBy, int pageIndex, int itemsPerPage);

    Task<Result<Person>> CreateAsync(SavePersonRequest request);

    Task<Result<Person>> UpdateAsync(Guid id, SavePersonRequest request);
}