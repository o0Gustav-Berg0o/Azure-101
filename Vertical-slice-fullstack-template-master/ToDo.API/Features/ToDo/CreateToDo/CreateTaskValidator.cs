using FluentValidation;

namespace ToDo.API.Features.ToDo.CreateToDo
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskRequestDTO>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
