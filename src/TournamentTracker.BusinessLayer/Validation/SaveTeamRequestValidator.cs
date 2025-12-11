using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.DataAccessLayer.Entities;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validation;

public class SaveTeamRequestValidator : AbstractValidator<SaveTeamRequest>
{
    private readonly IDataContext dataContext;

    public SaveTeamRequestValidator(IDataContext dataContext)
    {
        this.dataContext = dataContext;

        RuleFor(t => t.TournamentId).NotEmpty().MustAsync(TournamentMustExistAsync).WithMessage("This tournament does not exists");
        RuleFor(t => t.Name).NotEmpty();
    }

    private async Task<bool> TournamentMustExistAsync(Guid tournamentId, CancellationToken cancellationToken)
    {
        var query = dataContext.GetData<Tournament>();
        return await query.AnyAsync(t => t.Id == tournamentId, cancellationToken);
    }
}