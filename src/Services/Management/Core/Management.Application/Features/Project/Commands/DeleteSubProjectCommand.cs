using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteSubProjectCommand(Guid SubProjectId) : ICommand<Unit>;

public class DeleteSubProjectCommandHandler(
    IDocumentSession session,
    ILabApiService labApiService) : ICommandHandler<DeleteSubProjectCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(
        DeleteSubProjectCommand request,
        CancellationToken cancellationToken)
    {
        // Verify sub project exists
        var subProject = await session.LoadAsync<ProjectEntity>(request.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);
        
        // Delete all papers from Lab service
        foreach (var paperId in subProject.PaperIds)
        {
            await labApiService.DeletePaperAsync(paperId, cancellationToken);
        }
        
        // Delete sub project
        session.Delete(subProject);
        
        await session.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }

    #endregion
}