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
        try
        {
            var prize = await dataContext.GetAsync<Entities.Prize>(id);
            if (prize is not null)
            {
                dataContext.Delete(prize);
                await dataContext.SaveAsync();

                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No prize found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Prize>> GetAsync(Guid id)
    {
        var dbPrize = await dataContext.GetAsync<Entities.Prize>(id);
        if (dbPrize is not null)
        {
            var prize = mapper.Map<Prize>(dbPrize);
            return prize;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No prize found with id {id}");
    }

    public async Task<Result<IEnumerable<Prize>>> GetListAsync()
    {
        var prizes = await dataContext.GetData<Entities.Prize>().OrderBy(p => p.Name)
            .ProjectTo<Prize>(mapper.ConfigurationProvider)
            .ToListAsync();

        return prizes;
    }

    public async Task<Result<Prize>> CreateAsync(SavePrizeRequest request)
    {
        try
        {
            var prize = mapper.Map<Entities.Prize>(request);
            dataContext.Insert(prize);
            await dataContext.SaveAsync();

            var createdPrize = mapper.Map<Prize>(prize);
            return createdPrize;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Prize>> UpdateAsync(Guid id, SavePrizeRequest request)
    {
        try
        {
            var query = dataContext.GetData<Entities.Prize>(trackingChanges: false);
            var prize = await query.FirstOrDefaultAsync(p => p.Id == id);

            if (prize is not null)
            {
                mapper.Map(request, prize);
                await dataContext.SaveAsync();

                var savedPrize = mapper.Map<Prize>(prize);
                return savedPrize;
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No prize found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }
}