namespace TournamentTracker.Shared.Models.Requests;

public record class SavePersonRequest(Guid TeamId, string FirstName, string LastName, DateOnly BirthDate, string City, string CellphoneNumber, string EmailAddress);