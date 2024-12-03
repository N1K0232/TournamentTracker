using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TinyHelpers.Extensions;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.DataAccessLayer.Extensions;
using TournamentTracker.Shared.Models;
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
            if (tournament is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "No tournament found");
            }

            dataContext.Delete(tournament);
            await dataContext.SaveAsync();

            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Tournament>> GetAsync(Guid id)
    {
        var dbTournament = await dataContext.GetAsync<Entities.Tournament>(id);
        if (dbTournament is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No tournament found");
        }

        var tournament = mapper.Map<Tournament>(dbTournament);
        tournament.EnteredTeams = await GetTeamsAsync(id);
        tournament.Prizes = await GetPrizesAsync(id);

        return tournament;
    }

    public async Task<Result<PaginatedList<Tournament>>> GetListAsync(string name, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Tournament>()
            .WhereIf(name.HasValue(), t => t.Name.Contains(name));

        var totalCount = await query.CountAsync();
        var dbTournaments = await query.OrderBy(t => t.Name).ToListAsync(pageIndex, itemsPerPage);

        var tournaments = mapper.Map<IEnumerable<Tournament>>(dbTournaments).Take(itemsPerPage);
        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);

        await tournaments.ForEachAsync(async (tournament) =>
        {
            tournament.EnteredTeams = await GetTeamsAsync(tournament.Id);
            tournament.Prizes = await GetPrizesAsync(tournament.Id);
        });

        return new PaginatedList<Tournament>(tournaments, totalCount, hasNextPage);
    }

    public async Task<Result<Tournament>> InsertAsync(SaveTournamentRequest request)
    {
        var exists = await dataContext.GetData<Entities.Tournament>().AnyAsync(t => t.Name == request.Name && t.EntryFee == request.EntryFee && t.StartDate == request.StartDate);
        if (exists)
        {
            return Result.Fail(FailureReasons.Conflict, "This tournament already exists");
        }

        var dbTournament = mapper.Map<Entities.Tournament>(request);
        dataContext.Insert(dbTournament);

        await dataContext.SaveAsync();
        return mapper.Map<Tournament>(dbTournament);
    }

    public async Task<Result<Tournament>> UpdateAsync(Guid id, SaveTournamentRequest request)
    {
        try
        {
            var query = dataContext.GetData<Entities.Tournament>(true, true);
            var tournament = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (tournament is null)
            {
                return Result.Fail(FailureReasons.ItemNotFound, "No tournament found");
            }

            mapper.Map(request, tournament);
            await dataContext.SaveAsync();

            var savedTournament = mapper.Map<Tournament>(tournament);
            return savedTournament;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    private async Task<IEnumerable<Team>> GetTeamsAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Team>().Where(t => t.TournamentId == id);
        var teams = await query.ProjectTo<Team>(mapper.ConfigurationProvider).ToListAsync();

        await teams.ForEachAsync(async (team) =>
        {
            team.Members = await GetMembersAsync(team.Id);
        });

        return teams;
    }

    private async Task<IEnumerable<Person>> GetMembersAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Person>().Where(p => p.TeamId == id);
        var people = await query.ProjectTo<Person>(mapper.ConfigurationProvider).ToListAsync();

        return people;
    }

    private async Task<IEnumerable<Prize>> GetPrizesAsync(Guid id)
    {
        var query = dataContext.GetData<Entities.Prize>().Where(p => p.TournamentId == id);
        var prizes = await query.ProjectTo<Prize>(mapper.ConfigurationProvider).ToListAsync();

        return prizes;
    }
}