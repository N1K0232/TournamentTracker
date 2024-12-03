using FluentValidation;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validations;

public class SaveTeamRequestValidator : AbstractValidator<SaveTeamRequest>
{
    public SaveTeamRequestValidator()
    {
        RuleFor(t => t.Tournament)
            .NotEmpty()
            .WithMessage("the tournament is required");

        RuleFor(t => t.Name)
            .MaximumLength(100)
            .NotEmpty()
            .WithMessage("the team name is required");
    }
}