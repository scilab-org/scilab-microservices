using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Queries.GetJournalById;

public record GetJournalByIdQuery(Guid Id) : ICommand<GetJournalByIdResult>;

public class GetJournalByIdQueryValidator : AbstractValidator<GetJournalByIdQuery>
{
    public GetJournalByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage(MessageCode.JournalIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);
    }
}

public class GetJournalByIdQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
    IMapper mapper)
    : IRequestHandler<GetJournalByIdQuery, GetJournalByIdResult>
{
    public async Task<GetJournalByIdResult> Handle(GetJournalByIdQuery request, CancellationToken cancellationToken)
    {
        var journal = await session.LoadAsync<ConferenceJournalEntity>(request.Id, cancellationToken);

        if (journal == null)
            throw new NotFoundException(MessageCode.JournalIsNotExists, request.Id.ToString());

        var response = mapper.Map<JournalDto>(journal);

        var template = await session.LoadAsync<TemplateEntity>(journal.TemplateId, cancellationToken);

        response.TemplateCode = template?.Code ?? "N/A";

        var projects = new List<ProjectJournalInfo>();
        if (journal.ProjectIds != null && journal.ProjectIds.Count != 0)
        {
            var availableProjects =
                await managementApiService.GetProjectsByIdsAsync(journal.ProjectIds, cancellationToken);

            var projectMap = availableProjects.ToDictionary(x => x.Id, x => x);

            projects = journal.ProjectIds
                .Where(projectMap.ContainsKey)
                .Select(id => projectMap[id])
                .Select(project => new ProjectJournalInfo
                {
                    Id = project.Id,
                    Name = project.Name ?? string.Empty,
                    Code = project.Code ?? string.Empty
                })
                .ToList();
        }

        return new GetJournalByIdResult(response, projects);
    }
}