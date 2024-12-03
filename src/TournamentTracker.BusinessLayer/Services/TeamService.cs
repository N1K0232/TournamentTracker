using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.DataAccessLayer.Extensions;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class TeamService(IDataContext dataContext, IMapper mapper) : ITeamService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        var team = await dataContext.GetAsync<Entities.Team>(id);
        if (team is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No team found");
        }

        dataContext.Delete(team);
        await dataContext.SaveAsync();

        return Result.Ok();
    }

    public async Task<Result<Team>> GetAsync(Guid id)
    {
        var dbTeam = await dataContext.GetAsync<Entities.Team>(id);
        if (dbTeam is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No team found");
        }

        var team = mapper.Map<Team>(dbTeam);
        team.Members = await GetMembersAsync(id);

        return team;
    }

    public async Task<Result<PaginatedList<Team>>> GetListAsync(string name, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Team>().WhereIf(name.HasValue(), t => t.Name.Contains(name));
        var totalCount = await query.CountAsync();

        var dbTeams = await query.OrderBy(t => t.Name).ToListAsync(pageIndex, itemsPerPage);
        var teams = mapper.Map<IEnumerable<Team>>(dbTeams);

        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);
        await teams.ForEachAsync(async (team) =>
        {
            team.Members = await GetMembersAsync(team.Id);
        });

        return new PaginatedList<Team>(teams, totalCount, hasNextPage);
    }

    public async Task<Result<Team>> InsertAsync(SaveTeamRequest request)
    {
        var exists = await dataContext.GetData<Entities.Team>().AnyAsync(t => t.Name == request.Name);
        if (exists)
        {
            return Result.Fail(FailureReasons.Conflict, "This team already exists");
        }

        var tournament = await dataContext.GetData<Entities.Tournament>().FirstOrDefaultAsync(t => t.Name == request.Tournament);
        if (tournament is null)
        {
            return Result.Fail(FailureReasons.ClientError, "The tournament does not exists");
        }

        var dbTeam = mapper.Map<Entities.Team>(request);
        dbTeam.Tournament = tournament;

        dataContext.Insert(dbTeam);
        await dataContext.SaveAsync();

        var team = mapper.Map<Team>(dbTeam);
        return team;
    }

    public async Task<Result<Team>> UpdateAsync(Guid id, SaveTeamRequest request)
    {
        var query = dataContext.GetData<Entities.Team>(trackingChanges: true);
        var dbTeam = await query.FirstOrDefaultAsync(t => t.Id == id);

        if (dbTeam is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No team found");
        }

        var tournament = await dataContext.GetData<Entities.Tournament>().FirstOrDefaultAsync(t => t.Name == request.Tournament);
        if (tournament is null)
        {
            return Result.Fail(FailureReasons.ClientError, "The tournament does not exists");
        }

        mapper.Map(request, dbTeam);
        dbTeam.Tournament = tournament;

        await dataContext.SaveAsync();
        return mapper.Map<Team>(dbTeam);
    }

    private async Task<IEnumerable<Person>> GetMembersAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Person>().Where(p => p.TeamId == id);
        var people = await query.ProjectTo<Person>(mapper.ConfigurationProvider).ToListAsync();

        return people;
    }
}