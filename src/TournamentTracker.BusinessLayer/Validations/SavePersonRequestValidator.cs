using FluentValidation;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Validations;

public class SavePersonRequestValidator : AbstractValidator<SavePersonRequest>
{
    public SavePersonRequestValidator()
    {
        RuleFor(p => p.FirstName)
            .MaximumLength(256)
            .NotEmpty()
            .WithMessage("the first name is required");

        RuleFor(p => p.LastName)
            .MaximumLength(256)
            .NotEmpty()
            .WithMessage("the last name is required");

        RuleFor(p => p.BirthDate)
            .NotEmpty()
            .WithMessage("the birth date is required");

        RuleFor(p => p.CellphoneNumber)
            .MaximumLength(50)
            .NotEmpty()
            .WithMessage("the cellphone number is required");

        RuleFor(p => p.EmailAddress)
            .EmailAddress()
            .MaximumLength(512)
            .NotEmpty()
            .WithMessage("the email address is required");
    }
}