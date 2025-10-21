using FluentValidation;


namespace Flights.Application.Flights.Dtos
{
    public class CreateFlightDtoValidator : AbstractValidator<CreateFlightDto>
    {
        public CreateFlightDtoValidator()
        {
            RuleFor(x => x.Origin).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Destination).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Departure).NotEmpty();
            RuleFor(x => x.Arrival).NotEmpty();
            RuleFor(x => x).Must(x => x.Arrival > x.Departure)
            .WithMessage("Arrival must be later than Departure");
        }
    }


    public class UpdateFlightStatusDtoValidator : AbstractValidator<UpdateFlightStatusDto>
    {
        public UpdateFlightStatusDtoValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}