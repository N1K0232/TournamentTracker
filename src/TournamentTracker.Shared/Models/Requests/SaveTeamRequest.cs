namespace TournamentTracker.Shared.Models.Requests;

public record class SaveTeamRequest(Guid TournamentId, string Name);