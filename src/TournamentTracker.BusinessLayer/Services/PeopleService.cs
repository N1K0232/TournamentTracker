using Microsoft.EntityFrameworkCore;
using OperationResults;
using SimpleTransit;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Notifications;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class PeopleService(IDataContext dataContext, INotificationPublisher notificationPublisher) : IPeopleService
{
    public async Task<Result<Person>> CreateAsync(SavePersonRequest request, CancellationToken cancellationToken)
    {
        var person = new Entities.Person
        {
            TeamId = request.TeamId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            City = request.City,
            CellphoneNumber = request.CellphoneNumber,
            EmailAddress = request.EmailAddress
        };

        await dataContext.CreateAsync(person, cancellationToken);
        await dataContext.SaveAsync(cancellationToken);

        await notificationPublisher.NotifyAsync(new PersonCellphoneNotificationMessage(request.CellphoneNumber), cancellationToken);
        await notificationPublisher.NotifyAsync(new PersonEmailNotificationMessage(request.EmailAddress, request.TeamId), cancellationToken);

        var createdPerson = new Person(person.Id, request.FirstName, request.LastName, request.BirthDate, request.City, person.Team?.Name, request.CellphoneNumber, request.EmailAddress);
        return createdPerson;
    }

    public async Task<Result<Person>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbPerson = await dataContext.GetData<Entities.Person>().Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (dbPerson is not null)
        {
            var person = new Person(id, dbPerson.FirstName, dbPerson.LastName, dbPerson.BirthDate, dbPerson.City, dbPerson.Team.Name, dbPerson.CellphoneNumber, dbPerson.EmailAddress);
            return person;
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No person found", $"No person found with id {id}");
    }

    public async Task<Result<IEnumerable<Person>>> GetListAsync(CancellationToken cancellationToken)
    {
        var people = await dataContext.GetData<Entities.Person>()
            .Include(p => p.Team)
            .Select(p => new Person(p.Id, p.FirstName, p.LastName, p.BirthDate, p.City, p.Team.Name, p.CellphoneNumber, p.EmailAddress))
            .ToListAsync(cancellationToken);

        return people;
    }

    public async Task<Result> UpdateAsync(Guid id, SavePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await dataContext.GetData<Entities.Person>(true).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (person is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "No person found", $"No person found with id {id}");
        }

        person.TeamId = request.TeamId;
        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.BirthDate = request.BirthDate;
        person.City = request.City;

        if (person.CellphoneNumber != request.CellphoneNumber)
        {
            person.CellphoneNumber = request.CellphoneNumber;
            await notificationPublisher.NotifyAsync(new PersonCellphoneNotificationMessage(request.CellphoneNumber), cancellationToken);
        }

        if (person.EmailAddress != request.EmailAddress)
        {
            person.EmailAddress = request.EmailAddress;
            await notificationPublisher.NotifyAsync(new PersonEmailNotificationMessage(request.EmailAddress, request.TeamId), cancellationToken);
        }

        await dataContext.SaveAsync(cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var person = await dataContext.GetAsync<Entities.Person>(id, cancellationToken);
        if (person is not null)
        {
            await dataContext.DeleteAsync(person, cancellationToken);
            await dataContext.SaveAsync(cancellationToken);

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.ItemNotFound, "No person found", $"No person found with id {id}");
    }
}