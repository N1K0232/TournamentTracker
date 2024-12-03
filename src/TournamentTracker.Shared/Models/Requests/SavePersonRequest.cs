namespace TournamentTracker.Shared.Models.Requests;

public class SavePersonRequest
{
    public string Team { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateOnly BirthDate { get; set; }

    public string CellphoneNumber { get; set; }

    public string EmailAddress { get; set; }
}