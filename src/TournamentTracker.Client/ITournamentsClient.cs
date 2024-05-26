using Refit;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;

namespace TournamentTracker.Client;

public interface ITournamentsClient
{
    [Get("/api/tournaments")]
    Task<ApiResponse<ListResult<Tournament>>> GetListAsync(string name, int pageIndex, int itemsPerPage);

    [Get("/api/tournaments/{id}")]
    Task<ApiResponse<Tournament>> GetAsync(Guid id);
}