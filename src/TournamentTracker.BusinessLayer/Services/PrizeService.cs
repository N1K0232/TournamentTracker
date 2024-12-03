using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class PrizeService(IDataContext dataContext, IMapper mapper) : IPrizeService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        var prize = await dataContext.GetAsync<Entities.Prize>(id);
        if (prize is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No prize found");
        }

        dataContext.Delete(prize);
        await dataContext.SaveAsync();

        return Result.Ok();
    }

    public async Task<Result<Prize>> GetAsync(Guid id)
    {
        var dbPrize = await dataContext.GetAsync<Entities.Prize>(id);
        if (dbPrize is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No prize found");
        }

        var prize = mapper.Map<Prize>(dbPrize);
        return prize;
    }

    public async Task<Result<IEnumerable<Prize>>> GetListAsync()
    {
        var prizes = await dataContext.GetData<Entities.Prize>().OrderBy(p => p.Name)
            .ProjectTo<Prize>(mapper.ConfigurationProvider)
            .ToListAsync();

        return prizes;
    }

    public async Task<Result<Prize>> InsertAsync(SavePrizeRequest request)
    {
        var exists = await dataContext.GetData<Entities.Prize>().AnyAsync(p => p.Name == request.Name);
        if (exists)
        {
            return Result.Fail(FailureReasons.Conflict, "This prize already exists");
        }

        var tournament = await dataContext.GetData<Entities.Tournament>().FirstOrDefaultAsync(t => t.Name == request.Tournament);
        if (tournament is null)
        {
            return Result.Fail(FailureReasons.ClientError, "This tournament does not exists");
        }

        var prize = mapper.Map<Entities.Prize>(request);
        prize.Tournament = tournament;

        dataContext.Insert(prize);
        await dataContext.SaveAsync();

        var savedPrize = mapper.Map<Prize>(prize);
        return savedPrize;
    }

    public async Task<Result<Prize>> UpdateAsync(Guid id, SavePrizeRequest request)
    {
        var query = dataContext.GetData<Entities.Prize>(trackingChanges: false);
        var dbPrize = await query.FirstOrDefaultAsync(p => p.Id == id);

        if (dbPrize is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No prize found");
        }

        mapper.Map(request, dbPrize);
        await dataContext.SaveAsync();

        var savedPrize = mapper.Map<Prize>(dbPrize);
        return savedPrize;
    }
}