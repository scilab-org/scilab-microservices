using AutoMapper;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthorById;

public record GetPaperAuthorByIdQuery(Guid Id) : ICommand<GetPaperAuthorByIdResult>;

public class GetPaperAuthorByIdQueryValidator : AbstractValidator<GetPaperAuthorByIdQuery>
{
    public GetPaperAuthorByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetPaperAuthorByIdQueryHandler(
    IDocumentSession session,
    IMapper mapper,
    IManagementApiService managementApiService)
    : IRequestHandler<GetPaperAuthorByIdQuery, GetPaperAuthorByIdResult>
{
    public async Task<GetPaperAuthorByIdResult> Handle(GetPaperAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<PaperAuthorEntity>(request.Id, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        var result = mapper.Map<PaperAuthorDto>(entity);
        var role = await session.LoadAsync<AuthorRoleEntity>(entity.AuthorRoleId, cancellationToken);
        if (role is not null)
        {
            result.AuthorRoleName = role.Name;
            result.AuthorRoleDescription = role.Description;
        }
        await ApplyAffiliationDetailsAsync(result, cancellationToken);

        return new GetPaperAuthorByIdResult(result);
    }

    private async Task ApplyAffiliationDetailsAsync(PaperAuthorDto item, CancellationToken cancellationToken)
    {
        if (item.MemberId == Guid.Empty || item.AffiliationId == Guid.Empty)
            return;

        var member = await managementApiService.GetMemberByIdAsync(item.MemberId, cancellationToken);
        if (member is null)
            return;

        var affiliation = await managementApiService.GetUserAffiliationByUserIdAndAffiliationIdAsync(
            member.UserId,
            item.AffiliationId,
            cancellationToken);
        if (affiliation is null)
            return;

        item.Department = affiliation.Department;
        item.Position = affiliation.Position;
        item.AffiliationStartYear = affiliation.AffiliationStartYear;
        item.AffiliationEndYear = affiliation.AffiliationEndYear;
    }
}
