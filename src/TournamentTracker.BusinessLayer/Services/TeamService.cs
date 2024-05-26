using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.DataAccessLayer.Extensions;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class TeamService(IDataContext dataContext, IMapper mapper) : ITeamService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var team = await dataContext.GetAsync<Entities.Team>(id);
            if (team is not null)
            {
                dataContext.Delete(team);
                await dataContext.SaveAsync();

                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No team found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Team>> GetAsync(Guid id)
    {
        var dbTeam = await dataContext.GetAsync<Entities.Team>(id);
        if (dbTeam is not null)
        {
            var team = mapper.Map<Team>(dbTeam);
            team.Members = await GetMembersAsync(id);

            return team;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No team found with id {id}");
    }

    public async Task<Result<ListResult<Team>>> GetListAsync(string name, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Team>().WhereIf(name.HasValue(), t => t.Name.Contains(name));
        var totalCount = await query.LongCountAsync();
        var teams = await query.ProjectTo<Team>(mapper.ConfigurationProvider)
            .Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1)
            .ToListAsync();

        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);
        await teams.ForEachAsync(async (team) =>
        {
            team.Members = await GetMembersAsync(team.Id);
        });

        return new ListResult<Team>(teams.Take(itemsPerPage), totalCount, hasNextPage);
    }

    public async Task<Result<Team>> CreateAsync(SaveTeamRequest request)
    {
        try
        {
            var team = mapper.Map<Entities.Team>(request);
            dataContext.Insert(team);
            await dataContext.SaveAsync();

            var createdTeam = mapper.Map<Team>(team);
            return createdTeam;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Team>> UpdateAsync(Guid id, SaveTeamRequest request)
    {
        try
        {
            var query = dataContext.GetData<Entities.Team>(trackingChanges: true);
            var team = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (team is not null)
            {
                mapper.Map(request, team);
                await dataContext.SaveAsync();

                var savedTeam = mapper.Map<Team>(team);
                return savedTeam;
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No team found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    private async Task<ListResult<Person>> GetMembersAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Person>().Where(p => p.TeamId == id);
        var totalCount = await query.LongCountAsync();
        var people = await query.ProjectTo<Person>(mapper.ConfigurationProvider).ToListAsync();

        return new ListResult<Person>(people, totalCount);
    }
}