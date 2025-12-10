using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OperationResults;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class TournamentService(IDataContext dataContext) : ITournamentService
{
    public async Task<Result<Tournament>> CreateAsync(SaveTournamentRequest request, CancellationToken cancellationToken)
    {
        var tournament = new Entities.Tournament
        {
            Name = request.Name,
            EntryFee = request.EntryFee,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt
        };

        await dataContext.CreateAsync(tournament, cancellationToken);
        await dataContext.SaveAsync(cancellationToken);

        var createdTournament = new Tournament(tournament.Id, request.Name, request.EntryFee, request.StartsAt, request.EndsAt);
        return createdTournament;
    }

    public async Task<Result<Tournament>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbTournament = await dataContext.GetData<Entities.Tournament>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (dbTournament is not null)
        {
            var tournament = new Tournament(id, dbTournament.Name, dbTournament.EntryFee, dbTournament.StartsAt, dbTournament.EndsAt);
            return tournament;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No tournament found", $"No tournament found with id {id}");
    }

    public async Task<Result<IEnumerable<Tournament>>> GetListAsync(CancellationToken cancellationToken)
    {
        var tournaments = await dataContext.GetData<Entities.Tournament>()
            .Select(t => new Tournament(t.Id, t.Name, t.EntryFee, t.StartsAt, t.EndsAt))
            .ToListAsync(cancellationToken);

        return tournaments;
    }

    public async Task<Result> UpdateAsync(Guid id, SaveTournamentRequest request, CancellationToken cancellationToken)
    {
        var tournament = await dataContext.GetData<Entities.Tournament>(true).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tournament is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No tournament found", $"No tournament found with id {id}");
        }

        tournament.Name = request.Name;
        tournament.EntryFee = request.EntryFee;
        tournament.StartsAt = request.StartsAt;
        tournament.EndsAt = request.EndsAt;

        await dataContext.SaveAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var tournament = await dataContext.GetAsync<Entities.Tournament>(id, cancellationToken);
        if (tournament is not null)
        {
            await dataContext.DeleteAsync(tournament, cancellationToken);
            await dataContext.SaveAsync(cancellationToken);

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No tournament found", $"No tournament found with id {id}");
    }
}