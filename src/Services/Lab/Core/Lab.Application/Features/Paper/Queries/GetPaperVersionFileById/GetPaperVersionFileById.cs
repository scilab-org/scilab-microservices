using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetPaperVersionFileById;

public record GetPaperVersionFileByIdQuery(Guid Id) : ICommand<PaperVersionFileDto>;

public class GetPaperVersionFileByIdQueryValidator : AbstractValidator<GetPaperVersionFileByIdQuery>
{
    public GetPaperVersionFileByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PdfFileIdIsRequired);
    }
}

public class GetPaperVersionFileByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : ICommandHandler<GetPaperVersionFileByIdQuery, PaperVersionFileDto>
{
    public async Task<PaperVersionFileDto> Handle(
        GetPaperVersionFileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var pdf = await session.LoadAsync<PaperVersionFileEntity>(request.Id, cancellationToken)
                  ?? throw new NotFoundException(MessageCode.PdfFileNotFound, request.Id.ToString());

        return mapper.Map<PaperVersionFileDto>(pdf);
    }
}
