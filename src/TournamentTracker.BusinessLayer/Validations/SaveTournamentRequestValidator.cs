using FluentValidation;
using TinyHelpers.Extensions;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validations;

public class SaveTournamentRequestValidator : AbstractValidator<SaveTournamentRequest>
{
    public SaveTournamentRequestValidator()
    {
        RuleFor(t => t.Name)
            .MaximumLength(100)
            .NotEmpty()
            .WithMessage("the name is required");

        RuleFor(t => t.EntryFee)
            .PrecisionScale(8, 2, true)
            .WithMessage("insert a valid entry fee")
            .NotEmpty()
            .WithMessage("the entry fee is required");

        RuleFor(t => t.StartDate)
            .GreaterThan(DateTime.UtcNow.ToDateOnly())
            .WithMessage("you can't insert a past date");
    }
}