namespace TournamentTracker.Shared.Models.Requests;

public record class SaveTournamentRequest(string Name, decimal EntryFee, DateOnly StartDate);