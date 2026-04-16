using Lab.Application.Dtos.Sections;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Commands.UpdateGuideline;

public record UpdateGuidelineCommand(UpdateGuidelineDto Dto, Guid Id, Guid UserId, string UserName) : ICommand<Guid>;



public class UpdateGuidelineCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : ICommandHandler<UpdateGuidelineCommand, Guid>
{
    #region Implementation

    public async Task<Guid> Handle(UpdateGuidelineCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var section = await session.Query<SectionEntity>()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var memberInfo = await managementApiService.GetMemberByPaperIdAsync(
            section.PaperId,
            request.UserId,
            cancellationToken);

        if (memberInfo == null)
            throw new UnauthorizedException(MessageCode.AccessDenied);

        var (_, memberId, _) = memberInfo.Value;

        var contributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == section.PaperId
                        && x.MemberId == memberId
                        && (x.SectionId == section.Id || x.MarkSectionId == section.Id)) 
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new UnauthorizedException(MessageCode.AccessDenied);

        var relatedSectionIds = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == section.PaperId
                        && x.MarkSectionId == contributor.MarkSectionId
                        && x.SectionId.HasValue)
            .Select(x => x.SectionId!.Value)
            .ToListAsync(cancellationToken);

        var targetSectionIds = relatedSectionIds
            .Append(contributor.MarkSectionId)
            .Distinct()
            .ToList();

        var sections = await session.Query<SectionEntity>()
            .Where(x => targetSectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var currentSection in sections)
        {
            var sectionRule = SectionRuleComposer.BuildSectionRule(currentSection.Title, dto.Description, dto.MainIdea);
            var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(
                currentSection.ProjectRule,
                currentSection.PaperRule,
                sectionRule);

            currentSection.Update(
                description: dto.Description,
                mainIdea: dto.MainIdea,
                sectionRule: sectionRule,
                rule: normalizedRule);

            session.Update(currentSection);
        }

        await session.SaveChangesAsync(cancellationToken);

        return section.Id;
    }

    #endregion
}