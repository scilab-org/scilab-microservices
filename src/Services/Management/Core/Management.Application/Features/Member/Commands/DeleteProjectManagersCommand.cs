using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;
using Microsoft.AspNetCore.Http;

namespace Management.Application.Features.Member.Commands;

public record DeleteProjectManagersCommand(Guid ProjectId, DeleteProjectManagersDto Dto) : ICommand<List<Guid>>;

public class DeleteProjectManagersValidator : AbstractValidator<DeleteProjectManagersCommand>
{
    public DeleteProjectManagersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.MemberIds)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdsAreRequired);
    }
}

public class DeleteProjectManagersCommandHandler(
    IDocumentSession session)
    : ICommandHandler<DeleteProjectManagersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(DeleteProjectManagersCommand command, CancellationToken cancellationToken)
    {

        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        var memberIdSet = command.Dto.MemberIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToHashSet();

        if (memberIdSet.Count == 0)
            throw new ClientValidationException(MessageCode.MemberIdsAreRequired);

        var managersToRemove = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId
                        && memberIdSet.Contains(x.Id)
                        && x.ProjectRole == AuthorizeConstants.ProjectManager)
            .ToListAsync(cancellationToken);

        if (!managersToRemove.Any())
            throw new NotFoundException(MessageCode.MembersNotFound);

        await session.BeginTransactionAsync(cancellationToken);

        var deletedIds = new List<Guid>();
        foreach (var manager in managersToRemove)
        {
            session.Delete(manager);
            deletedIds.Add(manager.Id);
        }

        await session.SaveChangesAsync(cancellationToken);

        return deletedIds;
    }

    #endregion
}
