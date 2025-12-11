using Microsoft.EntityFrameworkCore;
using OperationResults;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class TeamService(IDataContext dataContext) : ITeamService
{
    public async Task<Result<Team>> CreateAsync(SaveTeamRequest request, CancellationToken cancellationToken)
    {
        var team = new Entities.Team
        {
            TournamentId = request.TournamentId,
            Name = request.Name
        };

        await dataContext.CreateAsync(team, cancellationToken);
        await dataContext.SaveAsync(cancellationToken);

        var createdTeam = new Team(team.Id, request.Name, team.Tournament?.Name);
        return createdTeam;
    }

    public async Task<Result<Team>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbTeam = await dataContext.GetData<Entities.Team>().Include(t => t.Tournament).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (dbTeam is not null)
        {
            var team = new Team(id, dbTeam.Name, dbTeam.Tournament.Name);
            return team;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No team found", $"No team found with id {id}");
    }

    public async Task<Result<IEnumerable<Team>>> GetListAsync(CancellationToken cancellationToken)
    {
        var teams = await dataContext.GetData<Entities.Team>()
            .Include(t => t.Tournament)
            .Select(t => new Team(t.Id, t.Name, t.Tournament.Name))
            .ToListAsync(cancellationToken);

        return teams;
    }

    public async Task<Result> UpdateAsync(Guid id, SaveTeamRequest request, CancellationToken cancellationToken)
    {
        var team = await dataContext.GetData<Entities.Team>(true).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (team is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No team found", $"No team found with id {id}");
        }

        team.TournamentId = request.TournamentId;
        team.Name = request.Name;

        await dataContext.SaveAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var team = await dataContext.GetAsync<Entities.Team>(id, cancellationToken);
        if (team is not null)
        {
            await dataContext.DeleteAsync(team, cancellationToken);
            await dataContext.SaveAsync(cancellationToken);

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No team found", $"No team found with id {id}");
    }
}