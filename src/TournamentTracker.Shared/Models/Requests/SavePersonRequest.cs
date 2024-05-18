using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTracker.Shared.Models.Requests;

public class SavePersonRequest
{
    public Guid TeamId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string CellphoneNumber { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;
}