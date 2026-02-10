using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Tag.Commands.CreateTag;

public record CreateTagCommand(string Name) : ICommand<Guid>;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(MessageCode.TagNameIsRequired)
            .MaximumLength(50).WithMessage(MessageCode.TagNameLengthExceeded)
            .NotNull().WithMessage(MessageCode.TagNameIsRequired)
            .Must(name => name != null && name.StartsWith($"#")).WithMessage(MessageCode.TagNameMustStartWithHash);
    }
}

public class CreateTagCommandHandler(IDocumentSession session) : IRequestHandler<CreateTagCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = TagEntity.Create(
            id: Guid.NewGuid(),
            name: request.Name);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}