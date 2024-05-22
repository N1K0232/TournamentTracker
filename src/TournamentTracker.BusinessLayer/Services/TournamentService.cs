using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class TournamentService(IDataContext dataContext, IMapper mapper) : ITournamentService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var tournament = await dataContext.GetAsync<Entities.Tournament>(id);
            if (tournament is not null)
            {
                dataContext.Delete(tournament);
                await dataContext.SaveAsync();

                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No tournament found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Tournament>> GetAsync(Guid id)
    {
        var dbTournament = await dataContext.GetAsync<Entities.Tournament>(id);
        if (dbTournament is not null)
        {
            var tournament = mapper.Map<Tournament>(dbTournament);
            tournament.EnteredTeams = await GetTeamsAsync(id);
            tournament.Prizes = await GetPrizesAsync(id);

            return tournament;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No tournament found with id {id}");
    }

    public async Task<Result<ListResult<Tournament>>> GetListAsync(string name, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Tournament>();

        if (name.HasValue())
        {
            query = query.Where(t => t.Name.Contains(name));
        }

        var totalCount = await query.LongCountAsync();
        var tournaments = await query.ProjectTo<Tournament>(mapper.ConfigurationProvider)
            .Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1)
            .ToListAsync();

        await tournaments.ForEachAsync(async (tournament) =>
        {
            tournament.EnteredTeams = await GetTeamsAsync(tournament.Id);
            tournament.Prizes = await GetPrizesAsync(tournament.Id);
        });

        var result = new ListResult<Tournament>
        {
            Content = tournaments.Take(itemsPerPage),
            TotalCount = totalCount,
            HasNextPage = tournaments.Count > itemsPerPage
        };

        return result;
    }

    public async Task<Result<Tournament>> CreateAsync(SaveTournamentRequest request)
    {
        try
        {
            var tournament = mapper.Map<Entities.Tournament>(request);
            dataContext.Insert(tournament);
            await dataContext.SaveAsync();

            var createdTournament = mapper.Map<Tournament>(tournament);
            return createdTournament;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Tournament>> UpdateAsync(Guid id, SaveTournamentRequest request)
    {
        try
        {
            var query = dataContext.GetData<Entities.Tournament>(true, true);
            var tournament = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (tournament is not null)
            {
                mapper.Map(request, tournament);
                await dataContext.SaveAsync();

                var savedTournament = mapper.Map<Tournament>(tournament);
                return savedTournament;
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No tournament found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    private async Task<ListResult<Team>> GetTeamsAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Team>().Where(t => t.TournamentId == id);
        var totalCount = await query.LongCountAsync();
        var teams = await query.ProjectTo<Team>(mapper.ConfigurationProvider).ToListAsync();

        await teams.ForEachAsync(async (team) =>
        {
            team.Members = await GetMembersAsync(team.Id);
        });

        var result = new ListResult<Team>
        {
            Content = teams,
            TotalCount = totalCount
        };

        return result;
    }

    private async Task<ListResult<Person>> GetMembersAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Person>().Where(p => p.TeamId == id);
        var totalCount = await query.LongCountAsync();
        var people = await query.ProjectTo<Person>(mapper.ConfigurationProvider).ToListAsync();

        var result = new ListResult<Person>
        {
            Content = people,
            TotalCount = totalCount
        };

        return result;
    }

    private async Task<IEnumerable<Prize>> GetPrizesAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Prize>().Where(p => p.TournamentId == id);
        var prizes = await query.ProjectTo<Prize>(mapper.ConfigurationProvider)
            .ToListAsync();

        return prizes;
    }
}