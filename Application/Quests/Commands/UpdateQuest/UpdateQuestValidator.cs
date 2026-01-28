using FluentValidation;

namespace Application.Quests.Commands.UpdateQuest;

public class UpdateQuestValidator : AbstractValidator<UpdateQuestCommand>
{
    public UpdateQuestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TargetCount).GreaterThan(0);
        
        RuleFor(x => x).Must(x => !x.StartAt.HasValue || !x.EndAt.HasValue || x.EndAt > x.StartAt)
            .WithMessage("EndAt must be after StartAt");
    }
}
