using AutoMapper;
using Lab.Application.Dtos.Sections;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetSectionsByPaperId;

public record GetSectionsByPaperIdQuery(Guid PaperId) : IQuery<GetSectionsByPaperIdResult>;

public sealed class GetSectionsByPaperIdQueryValidator : AbstractValidator<GetSectionsByPaperIdQuery>
{
    public GetSectionsByPaperIdQueryValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public sealed class GetSectionsByPaperIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetSectionsByPaperIdQuery, GetSectionsByPaperIdResult>
{
    public async Task<GetSectionsByPaperIdResult> Handle(
        GetSectionsByPaperIdQuery request,
        CancellationToken cancellationToken)
    {
        var sections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == request.PaperId && s.IsMainSection == true)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);


        var dtos = mapper.Map<List<SectionDto>>(sections);
        return new GetSectionsByPaperIdResult(dtos);
    }
}