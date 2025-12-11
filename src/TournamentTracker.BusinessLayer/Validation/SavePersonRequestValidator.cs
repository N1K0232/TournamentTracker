using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TinyHelpers.Extensions;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.DataAccessLayer.Entities;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validation;

public class SavePersonRequestValidator : AbstractValidator<SavePersonRequest>
{
    private readonly IDataContext dataContext;

    public SavePersonRequestValidator(IDataContext dataContext)
    {
        this.dataContext = dataContext;

        RuleFor(p => p.TeamId).NotEmpty().MustAsync(TeamMustExistAsync).WithMessage("This team does not exists");
        RuleFor(p => p.FirstName).NotEmpty();
        RuleFor(p => p.LastName).NotEmpty();
        RuleFor(p => p.BirthDate).InclusiveBetween(new DateTime(1970, 1, 1).ToDateOnly(), DateTime.Today.ToDateOnly());
        RuleFor(p => p.City).NotEmpty();
        RuleFor(p => p.CellphoneNumber).NotEmpty();
        RuleFor(p => p.EmailAddress).NotEmpty().EmailAddress();
    }

    private async Task<bool> TeamMustExistAsync(Guid teamId, CancellationToken cancellationToken)
    {
        var query = dataContext.GetData<Team>();
        return await query.AnyAsync(t => t.Id == teamId, cancellationToken);
    }
}