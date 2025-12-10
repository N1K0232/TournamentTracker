using FluentValidation;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validation;

public class SaveTournamentRequestValidator : AbstractValidator<SaveTournamentRequest>
{
    public SaveTournamentRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(t => t.Name).NotEmpty();
        RuleFor(t => t.EntryFee).PrecisionScale(6, 2, true);

        RuleFor(t => t.StartsAt).GreaterThanOrEqualTo(timeProvider.GetLocalNow());
        RuleFor(t => t.EndsAt).GreaterThan(t => t.StartsAt);
    }
}