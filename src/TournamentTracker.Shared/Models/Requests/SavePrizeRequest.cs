namespace TournamentTracker.Shared.Models.Requests;

public record class SavePrizeRequest(Guid TournamentId, string Name, decimal Value);