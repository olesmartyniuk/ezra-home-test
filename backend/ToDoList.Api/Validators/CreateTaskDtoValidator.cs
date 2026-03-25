using FluentValidation;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Due date must be today or in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
