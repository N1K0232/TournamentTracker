namespace TournamentTracker.Shared.Models.Requests;

public record class SavePrizeRequest(string Tournament, string Name, decimal Value);