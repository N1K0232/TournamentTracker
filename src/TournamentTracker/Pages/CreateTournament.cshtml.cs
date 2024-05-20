using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TournamentTracker.Pages;

public class CreateTournamentModel : PageModel
{
    private readonly ILogger<CreateTournamentModel> logger;

    public CreateTournamentModel(ILogger<CreateTournamentModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
    }
}