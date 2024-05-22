using FluentValidation;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validations;

public class SavePrizeRequestValidator : AbstractValidator<SavePrizeRequest>
{
    public SavePrizeRequestValidator()
    {
        RuleFor(p => p.TournamentId)
            .NotEmpty()
            .WithMessage("the tournament id is required");

        RuleFor(p => p.Name)
            .MaximumLength(100)
            .NotEmpty()
            .WithMessage("the name is required");

        RuleFor(p => p.Value)
            .PrecisionScale(8, 2, true)
            .WithMessage("insert a valid prize")
            .NotEmpty()
            .WithMessage("the prize is required");
    }
}