using JasperFx.Core;
using Lab.Application.Dtos.Sections;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Section.Commands.MarkSectionToCompleted;

public record MarkSectionToCompletedCommand(Guid Id, MarkSectionToCompletedDto Dto, string UserName) : ICommand<Guid>;

public class MarkSectionToCompletedCommandValidator : AbstractValidator<MarkSectionToCompletedCommand>
{
    public MarkSectionToCompletedCommandValidator()
    {
        RuleFor(command => command.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);
        RuleFor(command => command.Id)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);
        RuleFor(command => command.Dto.MemberId)
            .NotNull()
            .WithMessage(MessageCode.MemberIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdIsRequired);
        RuleFor(command => command.Dto.ProjectId)
            .NotNull()
            .WithMessage(MessageCode.ProjectIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class MarkSectionToCompletedCommandHandler(IDocumentSession session, IManagementApiService managementApiService)
    : ICommandHandler<MarkSectionToCompletedCommand, Guid>
{
    public async Task<Guid> Handle(MarkSectionToCompletedCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.Dto.ProjectId, cancellationToken);
        if (role.IsNullOrEmpty())
            throw new UnauthorizedException(MessageCode.AccessDenied);

        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, request.Id);

        var contributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == section.PaperId &&
                        x.MemberId == dto.MemberId &&
                        x.SectionId == section.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (contributor == null || contributor.SectionRole.EqualsIgnoreCase(AuthorizeConstants.SectionRead))
            throw new UnauthorizedException(MessageCode.AccessDenied);

        if (section.Status != SectionStatus.InProgress)
            throw new ClientValidationException(MessageCode.SectionStatusMustBeInReview);

        section.Update(status: SectionStatus.Completed, lastModifiedBy: request.UserName);
        session.Update(section);
        await session.SaveChangesAsync(cancellationToken);

        return section.Id;
    }
}