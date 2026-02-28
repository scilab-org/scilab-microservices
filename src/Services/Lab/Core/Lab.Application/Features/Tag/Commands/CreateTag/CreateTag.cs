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
            .NotNull().WithMessage(MessageCode.TagNameIsRequired);
    }
}

public class CreateTagCommandHandler(IDocumentSession session) : IRequestHandler<CreateTagCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

		var nomalizeName = request.Name.Trim().ToLowerInvariant();

        var existingTag = await session.Query<TagEntity>()
            .FirstOrDefaultAsync(x => x.Name == nomalizeName, cancellationToken);

        if (existingTag != null)
            throw new ClientValidationException(MessageCode.TagNameAlreadyExists, request.Name);

        var entity = TagEntity.Create(
            id: Guid.NewGuid(),
            name: nomalizeName);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}