using FluentValidation;

namespace Application.Quests.Commands.CreateQuest;

public class CreateQuestValidator : AbstractValidator<CreateQuestCommand>
{
    public CreateQuestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TargetCount).GreaterThan(0);
        
        RuleFor(x => x).Must(x => !x.StartAt.HasValue || !x.EndAt.HasValue || x.EndAt > x.StartAt)
            .WithMessage("EndAt must be after StartAt");
    }
}
