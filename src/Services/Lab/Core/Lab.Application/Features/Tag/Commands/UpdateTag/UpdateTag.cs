using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Tag.Commands.UpdateTag;

public record UpdateTagCommand(Guid Id, string? Name) : ICommand<Guid>;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(MessageCode.TagIdIsRequired);

        RuleFor(x => x.Name)
            .Must(name => name != null && name.StartsWith($"#")).WithMessage(MessageCode.TagNameMustStartWithHash);
    }
}

public class UpdateTagCommandHandler(IDocumentSession session) : IRequestHandler<UpdateTagCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<TagEntity>(request.Id, cancellationToken);

        if (entity == null)
            throw new ClientValidationException(MessageCode.TagIsNotExists, request.Id);

        entity.Update(
            name: request.Name);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}