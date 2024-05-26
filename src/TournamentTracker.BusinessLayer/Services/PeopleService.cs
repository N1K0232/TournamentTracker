using System.Linq.Dynamic.Core;
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

public class PeopleService(IDataContext dataContext, IMapper mapper) : IPeopleService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var person = await dataContext.GetAsync<Entities.Person>(id);
            if (person is not null)
            {
                dataContext.Delete(person);
                await dataContext.SaveAsync();

                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No person found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Person>> GetAsync(Guid id)
    {
        var dbPerson = await dataContext.GetAsync<Entities.Person>(id);
        if (dbPerson is not null)
        {
            var person = mapper.Map<Person>(dbPerson);
            return person;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No person found with id {id}");
    }

    public async Task<Result<ListResult<Person>>> GetListAsync(string name, string orderBy, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Person>()
            .WhereIf(name.HasValue(), p => p.FirstName.Contains(name) || p.LastName.Contains(name));

        var totalCount = await query.LongCountAsync();
        var people = await query.ProjectTo<Person>(mapper.ConfigurationProvider)
            .OrderBy(orderBy).Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1)
            .ToListAsync();

        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);
        return new ListResult<Person>(people.Take(itemsPerPage), totalCount);
    }

    public async Task<Result<Person>> CreateAsync(SavePersonRequest request)
    {
        try
        {
            var person = mapper.Map<Entities.Person>(request);
            dataContext.Insert(person);
            await dataContext.SaveAsync();

            var createdPerson = mapper.Map<Person>(person);
            return createdPerson;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Person>> UpdateAsync(Guid id, SavePersonRequest request)
    {
        try
        {
            var query = dataContext.GetData<Entities.Person>(trackingChanges: true);
            var person = await query.FirstOrDefaultAsync(p => p.Id == id);

            if (person is not null)
            {
                mapper.Map(request, person);
                await dataContext.SaveAsync();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No person found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }
}