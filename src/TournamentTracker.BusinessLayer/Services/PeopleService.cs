using System.Linq.Dynamic.Core;
using AutoMapper;
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

public class PeopleService(IDataContext dataContext, IMapper mapper) : IPeopleService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        var person = await dataContext.GetAsync<Entities.Person>(id);
        if (person is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No person found");
        }

        dataContext.Delete(person);
        await dataContext.SaveAsync();

        return Result.Ok();
    }

    public async Task<Result<Person>> GetAsync(Guid id)
    {
        var dbPerson = await dataContext.GetAsync<Entities.Person>(id);
        if (dbPerson is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No person found");
        }

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }

    public async Task<Result<PaginatedList<Person>>> GetListAsync(string firstName, string lastName, string orderBy, int pageIndex, int itemsPerPage)
    {
        var query = dataContext.GetData<Entities.Person>()
            .WhereIf(firstName.HasValue(), p => p.FirstName.Contains(firstName))
            .WhereIf(lastName.HasValue(), p => p.LastName.Contains(lastName));

        var totalCount = await query.CountAsync();
        var dbPeople = await query.OrderBy(orderBy).ToListAsync(pageIndex, itemsPerPage);

        var hasNextPage = await query.HasNextPageAsync(pageIndex, itemsPerPage);
        var people = mapper.Map<IEnumerable<Person>>(dbPeople).Take(itemsPerPage);

        return new PaginatedList<Person>(people, totalCount, hasNextPage);
    }

    public async Task<Result<Person>> InsertAsync(SavePersonRequest request)
    {
        var exists = await dataContext.GetData<Entities.Person>().AnyAsync(p => p.FirstName == request.FirstName && p.LastName == request.LastName);
        if (exists)
        {
            return Result.Fail(FailureReasons.Conflict, "This person already exists");
        }

        var team = await dataContext.GetData<Entities.Team>().FirstOrDefaultAsync(t => t.Name == request.Team);
        if (team is null)
        {
            return Result.Fail(FailureReasons.ClientError, "This team does not exists");
        }

        var dbPerson = mapper.Map<Entities.Person>(request);
        dbPerson.Team = team;

        dataContext.Insert(dbPerson);
        await dataContext.SaveAsync();

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }

    public async Task<Result<Person>> UpdateAsync(Guid id, SavePersonRequest request)
    {
        var query = dataContext.GetData<Entities.Person>(trackingChanges: true);
        var dbPerson = await query.FirstOrDefaultAsync(p => p.Id == id);

        if (dbPerson is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No person found");
        }

        var team = await dataContext.GetData<Entities.Team>().FirstOrDefaultAsync(t => t.Name == request.Team);
        if (team is null)
        {
            return Result.Fail(FailureReasons.ClientError, "This team does not exists");
        }

        mapper.Map(request, dbPerson);
        dbPerson.Team = team;

        await dataContext.SaveAsync();
        return mapper.Map<Person>(dbPerson);
    }
}