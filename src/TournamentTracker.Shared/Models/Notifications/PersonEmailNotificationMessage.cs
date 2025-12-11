namespace TournamentTracker.Shared.Models.Notifications;

public record class PersonEmailNotificationMessage(string EmailAddress, Guid TeamId);