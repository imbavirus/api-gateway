using FluentValidation;
using ApiGateway.Models.TelephonyServer;
using System.Text.RegularExpressions;

namespace ApiGateway.Models.Validators;

public class CallRequestValidator : AbstractValidator<CallRequest>
{
    // sample regex, this should be replaced with a more robust one if needed
    // This regex matches a phone number in the format allows for optional spaces, dashes, brackets and country code and longer numbers incase of international numbers
    // e.g. +123-456-7890, (123) 456-7890, 123 456 7890, 1234567890
    private static readonly Regex _phoneRegex = new Regex(@"^\(?(\+?[0-9]{1,4}[0-9]{1,4})\)?[- ]?([0-9]{3,5})[- ]?([0-9]{0,5})$", RegexOptions.Compiled);

    public CallRequestValidator()
    {
        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event name is required.");

        RuleFor(x => x.CallStart)
            .NotEmpty().WithMessage("Call start time is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Call start time cannot be in the future."); // Gives one day grace period incase of time zone issues

        RuleFor(x => x.CallId)
            .NotEmpty().WithMessage("Call ID is required and cannot be an empty GUID.");

        RuleFor(x => x.CallersName)
            .NotEmpty().WithMessage("Caller's name is required.")
            .MaximumLength(100).WithMessage("Caller's name cannot exceed 100 characters."); 

        RuleFor(x => x.CallersTelephoneNumber)
            .NotEmpty().WithMessage("Caller's telephone number is required.")
            .Matches(_phoneRegex).WithMessage("Caller's telephone number is not in a valid format.")
            .MaximumLength(20).WithMessage("Caller's telephone number cannot exceed 20 characters.");
    }
}
