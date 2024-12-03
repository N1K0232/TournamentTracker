using OperationResults;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Person>> GetAsync(Guid id);

    Task<Result<PaginatedList<Person>>> GetListAsync(string firstName, string lastName, string orderBy, int pageIndex, int itemsPerPage);

    Task<Result<Person>> InsertAsync(SavePersonRequest request);

    Task<Result<Person>> UpdateAsync(Guid id, SavePersonRequest request);
}