namespace TournamentTracker.Shared.Models;

public record class Tournament(Guid Id, string Name, decimal EntryFee, DateTimeOffset StartsAt, DateTimeOffset EndsAt);