namespace TournamentTracker.Shared.Models;

public record class Person(Guid Id, string FirstName, string LastName, DateOnly BirthDate, string City, string? Team, string CellphoneNumber, string EmailAddress);