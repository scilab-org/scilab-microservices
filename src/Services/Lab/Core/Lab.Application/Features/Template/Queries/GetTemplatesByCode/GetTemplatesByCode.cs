using AutoMapper;
using Lab.Application.Dtos.Template;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq;

namespace Lab.Application.Features.Template.Queries.GetTemplatesByCode;

public record GetTemplatesByCodeQuery(string Code) : IQuery<TemplateDto>;

public class GetTemplatesByCodeQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetTemplatesByCodeQuery, TemplateDto>
{
    #region Implementations

    public async Task<TemplateDto> Handle(GetTemplatesByCodeQuery request, CancellationToken cancellationToken)
    {
        var template = await session.Query<TemplateEntity>()
            .Where(x => x.Code == request.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (template is null)
            throw new NotFoundException(MessageCode.NotFound);

        return mapper.Map<TemplateDto>(template);
    }

    #endregion
}

