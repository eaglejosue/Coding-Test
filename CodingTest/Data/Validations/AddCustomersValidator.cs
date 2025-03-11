namespace Api.Data.Validations;

public class AddCustomersValidator : AbstractValidator<Customer>
{
    public AddCustomersValidator()
    {
        RuleFor(c => c.Id)
            .NotNull().NotEmpty().WithMessage("Id is required.");

        RuleFor(c => c.FirstName)
            .NotEmpty().WithMessage("First Name is required.");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("Last Name is required.");

        RuleFor(c => c.Age)
            .NotNull().NotEmpty().WithMessage("Age is required.")
            .GreaterThan(18).WithMessage("Age must be greater than 18.");
    }
}
