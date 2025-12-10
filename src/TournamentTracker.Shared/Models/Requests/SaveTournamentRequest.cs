namespace TournamentTracker.Shared.Models.Requests;

public record class SaveTournamentRequest(string Name, decimal EntryFee, DateTimeOffset StartsAt, DateTimeOffset EndsAt);